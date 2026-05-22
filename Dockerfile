# =========================
# Build Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["Seahawk WebAPI/Seahawk WebAPI.csproj", "Seahawk WebAPI/"]

COPY ["BackendServices/SeaHawkServices.Application/SeaHawkServices.Application.csproj", "BackendServices/SeaHawkServices.Application/"]

COPY ["BackendServices/SeaHawkServices.Domain/SeaHawkServices.Domain.csproj", "BackendServices/SeaHawkServices.Domain/"]

COPY ["BackendServices/SeaHawkServices.Infrastructure/SeaHawkServices.Infrastructure.csproj", "BackendServices/SeaHawkServices.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Seahawk WebAPI/Seahawk WebAPI.csproj"

# Copy all source code
COPY . .

# Publish API
WORKDIR "/src/Seahawk WebAPI"

RUN dotnet publish "Seahawk WebAPI.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# Runtime Stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Install wkhtmltopdf dependencies
RUN apt-get update && apt-get install -y \
    wkhtmltopdf \
    libgdiplus \
    libc6-dev

COPY --from=build /app/publish .

# Render PORT
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "Seahawk WebAPI.dll"]