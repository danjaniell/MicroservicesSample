using InventoryService.Features.Inventories.Queries;
using InventoryService.Features.Inventories.Services;
using SharedContracts.Contracts;
using System.Net;

namespace InventoryService.Features.Inventories.Endpoints;

public class GetAllInventories(IInventoryService inventoryService, ILogger<GetAllInventories> logger) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("", async () =>
            {
                logger.LogInformation("Attempting to retrieve all inventories.");
                var query = new GetAllInventoriesQuery();
                var result = await inventoryService.GetAllInventoriesAsync(query);

                logger.LogInformation("Successfully retrieved {Count} inventory items.", result.Count);
                return Results.Ok(result);
            })
            .Produces((int)HttpStatusCode.OK, typeof(IEnumerable<InventoryDto>))
            .WithSummary("Get All Inventory");
    }
}
