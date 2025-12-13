FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.sln ./
COPY SEM.API/*.csproj ./SEM.API/
COPY SEM.Domain/*.csproj ./SEM.Domain/
COPY SEM.Infrastructure/*.csproj ./SEM.Infrastructure/
RUN dotnet restore

COPY . ./

WORKDIR /app/SEM.API
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/SEM.API/out ./

EXPOSE 5140
EXPOSE 5141
EXPOSE 7279
EXPOSE 8080

ENTRYPOINT ["dotnet", "SEM.API.dll"]
