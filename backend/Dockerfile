FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /src

COPY SampleMonoRepo.DotNetApi/SampleMonoRepo.DotNetApi.csproj SampleMonoRepo.DotNetApi/
RUN dotnet restore SampleMonoRepo.DotNetApi/SampleMonoRepo.DotNetApi.csproj

COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_URLS=http://0.0.0.0:21000 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

EXPOSE 21000

ENTRYPOINT ["docker-entrypoint.sh"]
