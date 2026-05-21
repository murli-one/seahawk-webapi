# =========================
# Build Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["Seahawk WebAPI/Seahawk WebAPI.csproj", "Seahawk WebAPI/"]
COPY ["SeaHawkServices.Application/SeaHawkServices.Application.csproj", "SeaHawkServices.Application/"]
COPY ["SeaHawkServices.Domain/SeaHawkServices.Domain.csproj", "SeaHawkServices.Domain/"]
COPY ["SeaHawkServices.Infrastructure/SeaHawkServices.Infrastructure.csproj", "SeaHawkServices.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Seahawk WebAPI/Seahawk WebAPI.csproj"

# Copy full source code
COPY . .

# Publish API project
WORKDIR "/src/Seahawk WebAPI"
RUN dotnet publish "Seahawk WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Runtime Stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Render provides PORT automatically
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["dotnet", "Seahawk WebAPI.dll"]