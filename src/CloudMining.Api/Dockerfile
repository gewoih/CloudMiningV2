FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CloudMining.Api/CloudMining.Api.csproj", "src/CloudMining.Api/"]
COPY ["src/CloudMining.Application/CloudMining.Application.csproj", "src/CloudMining.Application/"]
COPY ["src/CloudMining.Domain/CloudMining.Domain.csproj", "src/CloudMining.Domain/"]
COPY ["src/CloudMining.Infrastructure/CloudMining.Infrastructure.csproj", "src/CloudMining.Infrastructure/"]
COPY ["src/CloudMining.Interfaces/CloudMining.Interfaces.csproj", "src/CloudMining.Interfaces/"]
RUN dotnet restore "src/CloudMining.Api/CloudMining.Api.csproj"
COPY . .
WORKDIR "/src/src/CloudMining.Api"
RUN dotnet build "CloudMining.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CloudMining.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudMining.Api.dll"]
