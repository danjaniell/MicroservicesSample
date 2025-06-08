using FluentValidation;
using InventoryService.Features.Inventories.Commands;
using InventoryService.Features.Inventories.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SharedContracts.Contracts;
using System.Net;

namespace InventoryService.Features.Inventories.Endpoints;

public class CreateInventory(IInventoryService inventoryService, ILogger<CreateInventory> logger) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("", async ([FromBody] CreateInventoryCommand request) =>
            {
                logger.LogInformation("Attempting to create inventory for product ID: {ProductId}", request.ProductId);

                try
                {
                    var newInventory = await inventoryService.CreateInventoryAsync(request);
                    logger.LogInformation("Successfully created inventory for product ID: {ProductId}", newInventory.ProductId);
                    return Results.Created(nameof(CreateInventory), newInventory);
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogError(ex, "Conflict during inventory creation for product ID {ProductId}.", request.ProductId);
                    return Results.Conflict(ex.Message);
                }
                catch (ValidationException ex)
                {
                    logger.LogError(ex, "Validation error during inventory creation: {Message}", ex.Message);
                    return Results.BadRequest(ex.Errors);
                }
            })
            .Produces((int)HttpStatusCode.Created, typeof(InventoryDto))
            .Produces((int)HttpStatusCode.BadRequest)
            .Produces((int)HttpStatusCode.Conflict)
            .WithOpenApi(operation => new OpenApiOperation
            {
                Summary = "Create Inventory",
                OperationId = "CreateInventory",
                Description = "Creates a new inventory record."
            });
    }
}
