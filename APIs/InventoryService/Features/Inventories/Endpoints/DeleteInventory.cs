using InventoryService.Features.Inventories.Commands;
using InventoryService.Features.Inventories.Services;
using System.Net;

namespace InventoryService.Features.Inventories.Endpoints;

public class DeleteInventory(IInventoryService inventoryService, ILogger<DeleteInventory> logger) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("{productId:guid}", async (Guid productId) =>
            {
                logger.LogInformation("Attempting to delete inventory for product ID: {ProductId}", productId);
                var command = new DeleteInventoryCommand(productId);

                try
                {
                    await inventoryService.DeleteInventoryAsync(command);
                    logger.LogInformation("Successfully deleted inventory for product ID: {ProductId}", productId);
                    return Results.NoContent();
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogError(ex, "Not found during inventory deletion for product ID {ProductId}.", productId);
                    return Results.NotFound(ex.Message);
                }
            })
            .Produces((int)HttpStatusCode.NoContent)
            .Produces((int)HttpStatusCode.NotFound)
            .WithSummary("Delete Inventory");
    }
}
