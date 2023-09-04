# Set the main image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env 
WORKDIR /app

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore

# Build and publish a release
RUN dotnet publish -c Release -o out

# Set the main image to launch
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Build runtime image
WORKDIR /app
COPY --from=build-env /app/out .

# Set entrypoint
ENTRYPOINT ["dotnet", "TelegaEngBot.dll"]
