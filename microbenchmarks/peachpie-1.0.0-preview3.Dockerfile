FROM mcr.microsoft.com/dotnet/sdk:5.0

COPY . /bench
WORKDIR /bench

RUN echo "{ \"msbuild-sdks\": { \"Peachpie.NET.Sdk\": \"1.0.0-preview3\" } }" > global.json
RUN dotnet build -c Release

ENTRYPOINT ["dotnet", "run", "--no-build", "-c", "Release"]
