FROM mcr.microsoft.com/dotnet/core/sdk:3.1

COPY ./config/peachpie /peachpie

# Restore NuGet packages before the specific app is questioned to better utilize Docker image caching
WORKDIR /peachpie
RUN echo "{ \"msbuild-sdks\": { \"Peachpie.NET.Sdk\": \"1.0.0-preview3\" } }" > global.json
RUN dotnet restore

ARG app

COPY ./apps/$app /app
WORKDIR /app
RUN ["/app/install.sh", "/peachpie/Website"]

WORKDIR /peachpie/Server
RUN dotnet build -c Release -f netcoreapp3.1
ENTRYPOINT ["dotnet", "run", "--no-build", "-c", "Release", "-f", "netcoreapp3.1"]
