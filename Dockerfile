# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project files
COPY ["src/shipments-api/Shipments.Domain/Shipments.Domain.csproj", "src/shipments-api/Shipments.Domain/"]
COPY ["src/shipments-api/Shipments.Application/Shipments.Application.csproj", "src/shipments-api/Shipments.Application/"]
COPY ["src/shipments-api/Shipments.Infrastructure/Shipments.Infrastructure.csproj", "src/shipments-api/Shipments.Infrastructure/"]
COPY ["src/shipments-api/Shipments.Api/Shipments.Api.csproj", "src/shipments-api/Shipments.Api/"]

# Restore dependencies
RUN dotnet restore "src/shipments-api/Shipments.Domain/Shipments.Domain.csproj"
RUN dotnet restore "src/shipments-api/Shipments.Application/Shipments.Application.csproj"
RUN dotnet restore "src/shipments-api/Shipments.Infrastructure/Shipments.Infrastructure.csproj"
RUN dotnet restore "src/shipments-api/Shipments.Api/Shipments.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/src/shipments-api/Shipments.Api"
RUN dotnet build "Shipments.Api.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "Shipments.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "Shipments.Api.dll"]
