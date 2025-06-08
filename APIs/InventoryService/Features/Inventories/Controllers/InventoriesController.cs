using Microsoft.AspNetCore.Mvc;
using InventoryService.Features.Inventories.Commands;
using InventoryService.Features.Inventories.Queries;
using InventoryService.Features.Inventories.Services;
using SharedContracts.Contracts;
using FluentValidation;
using System.Net;

namespace InventoryService.Features.Inventories.Controllers;

/// <summary>
/// API controller for managing inventory items.
/// </summary>
[ApiController]
[Route("api/inventories")]
public sealed class InventoriesController(IInventoryService inventoryService, ILogger<InventoriesController> logger) : ControllerBase
{

    /// <summary>
    /// Creates a new inventory item.
    /// </summary>
    /// <param name="request">The inventory creation request.</param>
    /// <returns>The newly created inventory DTO.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(InventoryDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<ActionResult<InventoryDto>> CreateInventory([FromBody] CreateInventoryCommand request)
    {
        logger.LogInformation("Attempting to create inventory for product ID: {ProductId}", request.ProductId);

        try
        {
            var newInventory = await inventoryService.CreateInventoryAsync(request);
            logger.LogInformation("Successfully created inventory for product ID: {ProductId}", newInventory.ProductId);
            return CreatedAtAction(nameof(GetInventoryById), new { productId = newInventory.ProductId }, newInventory);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Conflict during inventory creation for product ID {ProductId}.", request.ProductId);
            return Conflict(ex.Message); // 409 Conflict if item already exists
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Validation error during inventory creation: {Message}", ex.Message);
            return BadRequest(ex.Errors); // FluentValidation errors
        }
    }

    /// <summary>
    /// Updates an existing inventory item's quantity.
    /// </summary>
    /// <param name="request">The inventory update request.</param>
    /// <returns>The updated inventory DTO.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(InventoryDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<InventoryDto>> UpdateInventory([FromBody] UpdateInventoryCommand request)
    {
        logger.LogInformation("Attempting to update inventory for product ID: {ProductId}", request.ProductId);

        try
        {
            var updatedInventory = await inventoryService.UpdateInventoryAsync(request);
            logger.LogInformation("Successfully updated inventory for product ID: {ProductId}", updatedInventory.ProductId);
            return Ok(updatedInventory);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Not found during inventory update for product ID {ProductId}.", request.ProductId);
            return NotFound(ex.Message); // 404 Not Found if item does not exist
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Validation error during inventory update: {Message}", ex.Message);
            return BadRequest(ex.Errors); // FluentValidation errors
        }
    }

    /// <summary>
    /// Deletes an inventory item by its product ID.
    /// </summary>
    /// <param name="productId">The ID of the product to delete its inventory.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{productId:guid}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> DeleteInventory(Guid productId)
    {
        logger.LogInformation("Attempting to delete inventory for product ID: {ProductId}", productId);
        var command = new DeleteInventoryCommand(productId);

        try
        {
            await inventoryService.DeleteInventoryAsync(command);
            logger.LogInformation("Successfully deleted inventory for product ID: {ProductId}", productId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Not found during inventory deletion for product ID {ProductId}.", productId);
            return NotFound(ex.Message); // 404 Not Found if item does not exist
        }
    }
}
