FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Add Azure Artifacts Feed
ARG VERSION
ARG PAT

WORKDIR /app

# Copy all the csproj files and restore to cache an intermediate layer for faster builds
COPY src/*.sln ./ src/PrometheusMetrics.Api.Reference/*.csproj ./ ./Nuget.config ./

RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore 

# Copy everything else including binaries, config files etc.
COPY / .

# Build and Publish
FROM build AS publish

ARG VERSION

RUN dotnet build src/PrometheusMetrics.Api.Reference -c Release 
RUN dotnet publish src/PrometheusMetrics.Api.Reference -c Release -o /app/publish 

# Setup runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Disable invariant mode
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "PrometheusMetrics.Api.Reference.dll"]