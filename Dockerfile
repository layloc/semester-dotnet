FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY SEM.sln ./
COPY SEM.API/SEM.API.csproj SEM.API/
COPY SEM.Domain/SEM.Domain.csproj SEM.Domain/
COPY SEM.Infrastructure/SEM.Infrastructure.csproj SEM.Infrastructure/
COPY SEM.CodeGen/SEM.CodeGen.csproj SEM.CodeGen/
COPY SEM.Generators/SEM.Generators.csproj SEM.Generators/

RUN dotnet restore SEM.API/SEM.API.csproj

COPY . .
WORKDIR /src/SEM.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# /app is root-owned after COPY; ensure non-root user can write DataProtection keys
RUN mkdir -p /app/keys && chown -R app:app /app

ENV ASPNETCORE_ENVIRONMENT=Docker \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

HEALTHCHECK --interval=15s --timeout=5s --start-period=40s --retries=5 \
    CMD curl -f http://localhost:8080/login.html || exit 1

USER app

ENTRYPOINT ["dotnet", "SEM.API.dll"]
