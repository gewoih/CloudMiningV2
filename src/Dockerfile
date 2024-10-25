FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN apt-get update && \
    apt-get install -y --no-install-recommends ca-certificates && \
    rm -rf /var/lib/apt/lists/*

RUN update-ca-certificates

COPY ["CloudMining.Api/CloudMining.Api.csproj", "CloudMining.Api/"]
COPY ["CloudMining.Application/CloudMining.Application.csproj", "CloudMining.Application/"]
COPY ["CloudMining.Domain/CloudMining.Domain.csproj", "CloudMining.Domain/"]
COPY ["CloudMining.Infrastructure/CloudMining.Infrastructure.csproj", "CloudMining.Infrastructure/"]
COPY ["CloudMining.Interfaces/CloudMining.Interfaces.csproj", "CloudMining.Interfaces/"]
RUN dotnet restore "CloudMining.Api/CloudMining.Api.csproj"

COPY . .
WORKDIR "/src/CloudMining.Api"
RUN dotnet build "CloudMining.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CloudMining.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudMining.Api.dll"]
