param (
  [string] $serverDockerFilter='server-*',
  [string] $appFilter='*',
  [int] $seconds=30,
  [int] $concurrency=20,
  [string] $outputFile='results.json'
)

# Maximum theoretical number of requests to run (in practice it's limited by time instead)
$maxRequests = 10000000

# Common helper variables for consistent naming of images and containers
$network = "appbenchmarks"
$clientContainer = "${network}-client"
$serverContainer = "${network}-server"
$dbContainer = "${network}-db"
$clientTag = "${clientContainer}:latest"

# Helper function to create a reasonably named tag for the generated image
function Get-ServerTagName {
  param ($serverDockerFile, $appDir)
  $serverName = $serverDockerFile.Basename.ToLower()
  $appName = $appDir.Name.ToLower()
  return "appbenchmarks-${serverName}-app-${appName}:latest"
}

function Get-DbTagName {
  param ($appDir)
  $appName = $appDir.Name.ToLower()
  return "appbenchmarks-app-${appName}-db:latest"
}

# Helper function for parsing text information using Regex
function Select-Regex {
  param ([string]$subject, [string]$pattern)
  $m = $subject | Select-String -pattern $pattern
  return $m.Matches[0].Groups[1].Value
}

# Gather *.Dockerfile files of servers according to the filter
$serverDockerFiles = Get-ChildItem . -Filter $serverDockerFilter | Where-Object {$_.Name -like "server-*.Dockerfile"}

# Gather all the directories of all the apps to benchmark
$appDirs = Get-ChildItem ./apps -Filter $appFilter

# Generate all the docker images, one for each pair of server and app, and an optional database for each app
Foreach ($appDir in $appDirs) {
  $app = $appDir.Name

  Foreach ($serverDockerFile in $serverDockerFiles) {
    $tag = Get-ServerTagName $serverDockerFile $appDir
    & docker build --tag $tag --file $serverDockerFile --build-arg app=$app .
  }

  $dbDockerfile = "./apps/${app}/db.Dockerfile"
  if (Test-Path $dbDockerfile) {
    $dbTag = Get-DbTagName $appDir
    & docker build --tag $dbTag --file $dbDockerfile "./apps/${app}"
  }
}

# Generate the docker image for the benchmark client
& docker build --tag $clientTag --file ./client.Dockerfile .

# Create the network to enable communication between the containers
& docker network create $network

# Run all the apps, benchmark them and gather the results
$results = @{}
Foreach ($serverDockerFile in $serverDockerFiles) {
  $results[$serverDockerFile.Basename] = @{}
  Foreach ($appDir in $appDirs) {
    # Run the given container in the network
    $tag = Get-ServerTagName $serverDockerFile $appDir
    & docker run --name $serverContainer --network $network --detach --rm $tag

    # Run the client container in the network
    & docker run --name $clientContainer --network $network --detach --rm --tty $clientTag

    # Run the database container if its image is available
    $dbTag = Get-DbTagName $appDir
    $hasDb = & docker images -q $dbTag
    if ($hasDb) {
      & docker run --name $dbContainer --network $network --detach --rm $dbTag
    }

    # Compose the URL to run the benchmarks on
    $appDirFullPath = $appDir.Fullname
    $benchmarkConfig = Get-Content "${appDirFullPath}/benchmarkConfig.json" | ConvertFrom-Json
    $requestPath = $benchmarkConfig.requestPath
    $url = "http://${serverContainer}${requestPath}"

    # Wait for the server to start
    do {
      Write-Output "Waiting for server.."
      Start-Sleep -s 3
      & docker exec $clientContainer ab -n 1 $url
    } until ($?)

    # Wait for the DB to start
    if ($hasDb) {
      do {
        Write-Output "Waiting for DB.."
        Start-Sleep -s 3
        & docker exec --env MYSQL_PWD=password $clientContainer mysql -u root -h $dbContainer -e ";"
      } until ($?)
    }

    # Warm-up server (the client is reused to prevent from cooling it down due to reset)
    & docker exec $clientContainer ab -t $seconds -c $concurrency -n $maxRequests $url

    # Run the benchmarks and obtain the results
    $abOutput = (& docker exec $clientContainer ab -t $seconds -c $concurrency -n $maxRequests $url) | Out-String

    # Gather the results from the Apache Bench output
    $results[$serverDockerFile.Basename][$appDir.Name] = @{
      concurrency =         $concurrency;
      time_ms =             $seconds * 1000;
      requests =            (Select-Regex $abOutput "Complete requests:[ ]+([0-9]+)") -as [int];
      requests_failed =     (Select-Regex $abOutput "Failed requests:[ ]+([0-9]+)") -as [int];
      requests_per_second = (Select-Regex $abOutput "Requests per second:[ ]+([0-9]+(?:\.[0-9]+)?) \[#\/sec\] \(mean\)") -as [double];
      request_time_ms =     (Select-Regex $abOutput "Time per request:[ ]+([0-9]+(?:\.[0-9]+)?) \[ms\] \(mean\)") -as [double];
    }

    # Stop the containers (will be removed thanks to --rm on start)
    & docker stop -t 3 $clientContainer $serverContainer $dbContainer
  }
}

# Export the results
ConvertTo-Json -Compress $results | Out-File $outputFile

# Clean up
& docker network rm $network
