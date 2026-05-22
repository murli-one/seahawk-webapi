# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy csproj files
COPY ["Seahawk WebAPI/Seahawk WebAPI.csproj", "Seahawk WebAPI/"]
COPY ["BackendServices/SeaHawkServices.Application/SeaHawkServices.Application.csproj", "BackendServices/SeaHawkServices.Application/"]
COPY ["BackendServices/SeaHawkServices.Domain/SeaHawkServices.Domain.csproj", "BackendServices/SeaHawkServices.Domain/"]
COPY ["BackendServices/SeaHawkServices.Infrastructure/SeaHawkServices.Infrastructure.csproj", "BackendServices/SeaHawkServices.Infrastructure/"]

# Restore
RUN dotnet restore "Seahawk WebAPI/Seahawk WebAPI.csproj"

# Copy everything
COPY . .

WORKDIR "/src/Seahawk WebAPI"

# Publish
RUN dotnet publish "Seahawk WebAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Seahawk WebAPI.dll"]