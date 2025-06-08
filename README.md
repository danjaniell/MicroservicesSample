# 🏗️ Microservices Sample Solution

A .NET 9 microservices solution demonstrating clean architecture, API gateways, and service communication patterns.

## 🏛️ Solution Structure

```
MicroservicesSample/
├── APIs/
│   ├── InventoryService/       # Inventory management microservice
│   ├── ProductCatalogService/  # Product catalog microservice
│   └── Shared/                 # Shared API components
├── ApiGateway/                 # Ocelot API Gateway
└── SharedContracts/            # Shared DTOs and contracts
```

## 🛠️ Key Technologies & Packages

### Core

- [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0) - Cross-platform development platform
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core) - Web framework
- [Ocelot](https://github.com/ThreeMammals/Ocelot) - API Gateway

### Data & Validation

- [FluentValidation](https://docs.fluentvalidation.net/) - Validation library
- MemoryCache as Database
- [Bogus](https://github.com/bchavez/Bogus) - Data seeder

### API & Documentation

- [Scalar](https://github.com/scalar/scalar) - API documentation UI

## 🌐 Services Overview

### 🏪 Product Catalog Service (Port 5106)

Manages product information and catalog data.

- `GET /api/v1/products` - List all products
- `GET /api/v1/products/{id}` - Get product by ID
- `POST /api/v1/products` - Create new product
- `PUT /api/v1/products/{id}` - Update product
- `DELETE /api/v1/products/{id}` - Delete product

### 📦 Inventory Service (Port 5105)

Manages product inventory and stock levels.

- `GET /api/v1/inventories` - List all inventory items
- `GET /api/v1/inventories/{productId}` - Get inventory by product ID
- `POST /api/v1/inventories` - Create inventory record
- `PUT /api/v1/inventories` - Update inventory
- `DELETE /api/v1/inventories/{productId}` - Delete inventory record

### 🌉 API Gateway (Port 5165)

Single entry point for all client requests with request routing and composition.

- Routes requests to appropriate services
- Handles load balancing
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
   - API Gateway: http://localhost:5165
   - Product Catalog Scalar: http://localhost:5106/scalar
   - Inventory Service Scalar: http://localhost:5105/scalar

## 🔧 Development

### Project Structure

- **APIs/**: Contains individual microservices
- **ApiGateway/**: Ocelot API Gateway configuration
- **SharedContracts/**: Shared DTOs and interfaces

### Key Features

- Microservice Architecture
- Minimal API endpoints
- API versioning
- Health checks
- Clean architecture
- Container support
