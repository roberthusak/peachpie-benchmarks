& docker run --name wordpress-experiment-db -e MYSQL_ROOT_PASSWORD=root --mount type=bind,source=$pwd/init,destination=/docker-entrypoint-initdb.d -p 3306:3306 --rm mysql:8.0.20
