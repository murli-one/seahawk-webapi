# =========================
# BUILD STAGE
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

# Copy all files
COPY . .

WORKDIR "/src/Seahawk WebAPI"

# Publish app
RUN dotnet publish "Seahawk WebAPI.csproj" -c Release -o /app/publish

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Install wkhtmltopdf dependencies for Linux
RUN apt-get update && apt-get install -y \
    wkhtmltopdf \
    libgdiplus \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=build /app/publish .

# Render uses 10000 internally sometimes
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Seahawk WebAPI.dll"]