# Stage 1: Build base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy all project files for both APIs
COPY ["src/shipments-api/Shipments.Domain/Shipments.Domain.csproj", "src/shipments-api/Shipments.Domain/"]
COPY ["src/shipments-api/Shipments.Application/Shipments.Application.csproj", "src/shipments-api/Shipments.Application/"]
COPY ["src/shipments-api/Shipments.Infrastructure/Shipments.Infrastructure.csproj", "src/shipments-api/Shipments.Infrastructure/"]
COPY ["src/shipments-api/Shipments.Api/Shipments.Api.csproj", "src/shipments-api/Shipments.Api/"]

COPY ["src/costs-api/Costs.Domain/Costs.Domain.csproj", "src/costs-api/Costs.Domain/"]
COPY ["src/costs-api/Costs.Application/Costs.Application.csproj", "src/costs-api/Costs.Application/"]
COPY ["src/costs-api/Costs.Infrastructure/Costs.Infrastructure.csproj", "src/costs-api/Costs.Infrastructure/"]
COPY ["src/costs-api/Costs.Api/Costs.Api.csproj", "src/costs-api/Costs.Api/"]

# Restore dependencies for shipments-api
RUN dotnet restore "src/shipments-api/Shipments.Domain/Shipments.Domain.csproj"
RUN dotnet restore "src/shipments-api/Shipments.Application/Shipments.Application.csproj"
RUN dotnet restore "src/shipments-api/Shipments.Infrastructure/Shipments.Infrastructure.csproj"
RUN dotnet restore "src/shipments-api/Shipments.Api/Shipments.Api.csproj"

# Restore dependencies for costs-api
RUN dotnet restore "src/costs-api/Costs.Domain/Costs.Domain.csproj"
RUN dotnet restore "src/costs-api/Costs.Application/Costs.Application.csproj"
RUN dotnet restore "src/costs-api/Costs.Infrastructure/Costs.Infrastructure.csproj"
RUN dotnet restore "src/costs-api/Costs.Api/Costs.Api.csproj"

# Copy the rest of the source code
COPY . .

# Stage 2: Build shipments-api specifically
FROM build AS shipments-build
WORKDIR "/src/src/shipments-api/Shipments.Api"
RUN dotnet build "Shipments.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Shipments.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Build costs-api specifically
FROM build AS costs-build
WORKDIR "/src/src/costs-api/Costs.Api"
RUN dotnet build "Costs.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Costs.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Runtime for shipments-api
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS shipments

WORKDIR /app

# Copy the published Shipments application from the build stage
COPY --from=shipments-build /app/publish .

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

# Stage 5: Runtime for costs-api
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS costs

WORKDIR /app

# Copy the published Costs application from the build stage
COPY --from=costs-build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "Costs.Api.dll"]

