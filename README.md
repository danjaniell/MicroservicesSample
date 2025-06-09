# 🏗️ Microservices Sample Solution

A .NET 9 microservices solution demonstrating clean architecture, API gateways, and service communication patterns.

## 🏛️ Solution Structure

```
MicroservicesSample/
├── APIs/
│   ├── InventoryService/       # Inventory management microservice
│   ├── ProductCatalogService/  # Product catalog microservice
│   └── Shared/                 # Shared API components
├── ApiGateway/
│   ├── OcelotApiGateway/       # Ocelot API Gateway
│   ├── YARPGateway/            # YARP API Gateway
└── SharedContracts/            # Shared DTOs and contracts
```

## 🛠️ Key Technologies & Packages

### Core

- [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0) - Cross-platform development platform
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core) - Web framework
- [Ocelot](https://github.com/ThreeMammals/Ocelot) - API Gateway
- [YARP](https://github.com/dotnet/yarp) - API Gateway
- [Resilience](https://www.nuget.org/packages/Microsoft.Extensions.Resilience) - Microsoft Resilience Extensions (Polly)

### Data & Validation

- [FluentValidation](https://docs.fluentvalidation.net/) - Validation library
- MemoryCache as Database
- [Bogus](https://github.com/bchavez/Bogus) - Data seeder

### API & Documentation

- [Scalar](https://github.com/scalar/scalar) - API documentation UI

## 🌐 Services Overview

### [🏪 Product Catalog Service (Port 5106)](https://github.com/danjaniell/InventoryService)

### [📦 Inventory Service (Port 5105)](https://github.com/danjaniell/ProductCatalogService)

### 🌉 API Gateway (Port 5165/5166)

Single entry point for all client requests with request routing and composition.

- Routes requests to appropriate services
- Handles load balancing, rate limiting
- Provides API composition

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/) (optional for containerization)

### Running Locally

1. **Start the services**

   ```bash
   docker compose up
   ```

   or

   ```bash
   # Start API Gateway
   cd ApiGateway/OcelotApiGateway
   dotnet run

   # Start Product Catalog Service
   cd ../../APIs/ProductCatalogService
   dotnet run

   # Start Inventory Service
   cd ../InventoryService
   dotnet run
   ```

2. **Access the services**
   - API Gateway (Ocelot): http://localhost:5165
   - API Gateway (YARP): http://localhost:5166
   - Product Catalog Scalar: http://localhost:5106/scalar
   - Inventory Service Scalar: http://localhost:5105/scalar

## 🔧 Development

### Project Structure

- **APIs/**: Contains individual microservices
- **ApiGateway/**: Contains API Gateway projects
- **SharedContracts/**: Shared DTOs and interfaces

### Key Features

- Microservice Architecture
- Minimal API endpoints
- API versioning
- Health checks
- Clean architecture
- Container support
