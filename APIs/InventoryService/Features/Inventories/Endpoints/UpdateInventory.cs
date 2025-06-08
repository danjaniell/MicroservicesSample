using FluentValidation;
using InventoryService.Features.Inventories.Commands;
using InventoryService.Features.Inventories.Services;
using Microsoft.AspNetCore.Mvc;
using SharedContracts.Contracts;
using System.Net;

namespace InventoryService.Features.Inventories.Endpoints;

public class UpdateInventory(IInventoryService inventoryService, ILogger<UpdateInventory> logger) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("", async ([FromBody] UpdateInventoryCommand request) =>
            {
                logger.LogInformation("Attempting to update inventory for product ID: {ProductId}", request.ProductId);

                try
                {
                    var updatedInventory = await inventoryService.UpdateInventoryAsync(request);
                    logger.LogInformation("Successfully updated inventory for product ID: {ProductId}", updatedInventory.ProductId);
                    return Results.Ok(updatedInventory);
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogError(ex, "Not found during inventory update for product ID {ProductId}.", request.ProductId);
                    return Results.NotFound(ex.Message);
                }
                catch (ValidationException ex)
                {
                    logger.LogError(ex, "Validation error during inventory update: {Message}", ex.Message);
                    return Results.BadRequest(ex.Errors);
                }
            })
            .Produces((int)HttpStatusCode.OK, typeof(InventoryDto))
            .Produces((int)HttpStatusCode.BadRequest)
            .Produces((int)HttpStatusCode.NotFound);
    }
}
