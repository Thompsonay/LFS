# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and restore dependencies
COPY *.sln .
COPY LFSUiApp/*.csproj ./LFSUiApp/
RUN dotnet restore

# Copy everything else and publish
COPY LFSUiApp/. ./LFSUiApp/
WORKDIR /app/LFSUiApp
RUN dotnet publish -c Release -o out

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/LFSUiApp/out ./

# Start the app
ENTRYPOINT ["dotnet", "LFSUiApp.dll"]
