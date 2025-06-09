var builder = DistributedApplication.CreateBuilder(args);

var inventoryService = builder.AddProject<Projects.InventoryService>("inventory-service");
var productsService = builder.AddProject<Projects.ProductCatalogService>("products-service");

builder.AddProject<Projects.OcelotApiGateway>("ocelot-api-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(inventoryService)
    .WaitFor(inventoryService)
    .WithReference(productsService)
    .WaitFor(productsService);

builder.AddProject<Projects.YARPGateway>("yarp-api-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(inventoryService)
    .WaitFor(inventoryService)
    .WithReference(productsService)
    .WaitFor(productsService);

builder.Build().Run();
