# PostgreSQL + Entity Framework Integration Setup

## Overview
This document describes the PostgreSQL and Entity Framework Core integration that has been implemented for the Shipments API.

## Changes Made

### 1. Infrastructure Project Dependencies
Added the following NuGet packages to `Shipments.Infrastructure.csproj`:
- **Npgsql.EntityFrameworkCore.PostgreSQL** (8.0.0): PostgreSQL provider for EF Core
- **Microsoft.EntityFrameworkCore** (8.0.0): EF Core framework
- **Microsoft.EntityFrameworkCore.Tools** (8.0.0): CLI tools for migrations

### 2. Database Context
Created `Shipments.Infrastructure/Persistence/ShipmentsDbContext.cs`:
- Maps `Shipment` entity with `Dimensions` as an owned type
- Configures PostgreSQL-specific column types (uuid, timestamp with time zone)
- Creates indexes on `Status`, `DateCreated`, and composite `(Status, DateCreated)`
- Sets up proper constraints and validation

### 3. Entity Framework Repository
Created `Shipments.Infrastructure/Persistence/ShipmentEFRepository.cs`:
- Implements `IShipmentRepository` interface
- Provides async CRUD operations using EF Core LINQ
- Supports filtering by status and pagination
- Replaces the in-memory repository implementation

### 4. Dependency Injection Updates
Modified `Shipments.Infrastructure/DependencyInjection.cs`:
- Now accepts `IConfiguration` parameter
- Registers `ShipmentsDbContext` with PostgreSQL connection string
- Swaps repository implementation from in-memory to Entity Framework

### 5. API Configuration
Updated `Shipments.Api/Program.cs`:
- Passes configuration to infrastructure layer
- Automatically applies database migrations on startup
- Adds imports for EF Core and DbContext

### 6. Database Initialization
Updated `appsettings.json` files:
- **appsettings.json**: Production connection string (uses `db` hostname for Docker)
- **appsettings.Development.json**: Local development connection string (uses `localhost`)

### 7. Entity Framework Migrations
Created initial migration `InitialCreate`:
- Located in `Shipments.Infrastructure/Migrations/`
- Creates `shipments` table with proper schema
- Sets up indexes for performance optimization
- UUID as primary key with auto-generation

### 8. Docker Configuration
Created `Dockerfile` (multi-stage build):
- **Build stage**: Compiles the .NET application
- **Runtime stage**: Uses minimal .NET 8 runtime image
- Exposes port 8080
- Includes health check endpoint

Created `docker-compose.yml`:
- **PostgreSQL service** (postgres:16-alpine)
  - Port: 5432
  - Volume: `postgres_data` for data persistence
  - Environment: `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`
  - Health check included
  
- **API service**
  - Builds from Dockerfile
  - Port: 8080 (mapped to container 8080)
  - Depends on PostgreSQL being healthy
  - Environment variables for connection string
  - Health check endpoint

- **Network**: Custom docker network for service communication
- **Volume**: Persistent PostgreSQL data storage

## Getting Started

### Prerequisites
- Docker and Docker Compose installed
- .NET 8 SDK (for local development without Docker)

### Running with Docker Compose

```powershell
# Navigate to project root
cd e:\Programacao\Projetos\shipments

# Start services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

### API Access
- **Swagger UI**: http://localhost:8080/swagger
- **Health Check**: http://localhost:8080/health
- **API Base**: http://localhost:8080

### Local Development (Without Docker)

1. Install PostgreSQL locally (or use Docker for just the database)

2. Update `appsettings.Development.json`:
   ```json
   "DefaultConnection": "Host=localhost;Port=5432;Database=shipments_dev;Username=postgres;Password=postgres"
   ```

3. Create the database:
   ```powershell
   cd src\Shipments.Api
   dotnet build
   dotnet run
   ```

4. Database migrations run automatically on startup

## Database Operations

### View Database
```powershell
# Connect to PostgreSQL container
docker exec -it shipments-db psql -U postgres -d shipments

# List tables
\dt

# View shipments table
SELECT * FROM shipments;
```

### Create a New Migration
```powershell
cd src\Shipments.Api
dotnet ef migrations add MigrationName --project ..\Shipments.Infrastructure --startup-project .
```

### Remove Latest Migration
```powershell
cd src\Shipments.Api
dotnet ef migrations remove --project ..\Shipments.Infrastructure --startup-project .
```

## Architecture

```
Shipments.Api (WebAPI)
    ↓
Shipments.Application (Business Logic, Validators, Use Cases)
    ↓
Shipments.Infrastructure (EF Core, Repository Pattern)
    ↓
Shipments.Domain (Models, Business Rules)
    ↓
PostgreSQL Database
```

## Key Improvements

1. **Persistence**: Data now survives application restarts
2. **Scalability**: PostgreSQL supports concurrent connections and large datasets
3. **Query Optimization**: Indexes improve query performance
4. **Migration Management**: EF Core provides version control for schema changes
5. **Type Safety**: LINQ queries are compile-time checked
6. **Container Ready**: Docker setup enables consistent development and production environments

## Connection String Format

PostgreSQL connection string format:
```
Host=<hostname>;Port=<port>;Database=<database>;Username=<user>;Password=<password>
```

- **Docker**: `Host=db;Port=5432;Database=shipments;Username=postgres;Password=postgres`
- **Local**: `Host=localhost;Port=5432;Database=shipments_dev;Username=postgres;Password=postgres`

## Troubleshooting

### Container won't start
```powershell
# Check logs
docker-compose logs db
docker-compose logs api

# Rebuild images
docker-compose up --build
```

### Migration errors
```powershell
# Verify database connection
docker exec -it shipments-db pg_isready

# Check migration status
cd src\Shipments.Api
dotnet ef migrations list --project ..\Shipments.Infrastructure
```

### Port conflicts
- Change API port in `docker-compose.yml`: `"8081:8080"`
- Change PostgreSQL port: `"5433:5432"`

## Next Steps

1. **Testing**: Run unit and integration tests
2. **More Migrations**: Create migrations for new features
3. **Optimization**: Add more indexes based on query patterns
4. **Backup Strategy**: Configure PostgreSQL backup procedures
5. **Monitoring**: Integrate observability and logging

---

**Last Updated**: March 4, 2026  
**Contacts**: Infrastructure Team
