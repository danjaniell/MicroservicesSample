using InventoryService.Features.Inventories.Queries;
using InventoryService.Features.Inventories.Services;
using SharedContracts.Contracts;
using System.Net;

namespace InventoryService.Features.Inventories.Endpoints;

public class GetInventory(IInventoryService inventoryService, ILogger<GetInventory> logger) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("{productId:guid}", async (Guid productId) =>
            {
                logger.LogInformation("Attempting to retrieve inventory for product ID: {ProductId}", productId);
                var query = new GetInventoryByIdQuery(productId);
                var result = await inventoryService.GetInventoryByIdAsync(query);

                if (result == null)
                {
                    logger.LogWarning("Inventory for product ID {ProductId} not found.", productId);
                    return Results.NotFound();
                }

                logger.LogInformation("Successfully retrieved inventory for product ID: {ProductId}", productId);
                return Results.Ok(result);
            })
            .Produces((int)HttpStatusCode.OK, typeof(InventoryDto))
            .Produces((int)HttpStatusCode.NotFound)
            .WithSummary("Get Inventory");
    }
}
