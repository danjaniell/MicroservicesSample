using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalogService.Features.Products.Commands;
using ProductCatalogService.Features.Products.Queries;
using ProductCatalogService.Features.Products.Services;
using SharedContracts.Contracts;
using FluentValidation;
using System.Net;

namespace ProductCatalogService.Features.Products.Controllers;

/// <summary>
/// API controller for managing product catalog items.
/// </summary>
[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductService productService, ILogger<ProductsController> logger) : ControllerBase
{
    /// <summary>
    /// Gets a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product.</param>
    /// <returns>A product DTO.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        logger.LogInformation("Attempting to retrieve product with ID: {ProductId}", id);
        var query = new GetProductByIdQuery(id);
        var result = await productService.GetProductByIdAsync(query);

        if (result == null)
        {
            logger.LogWarning("Product with ID {ProductId} not found.", id);
            return NotFound();
        }

        logger.LogInformation("Successfully retrieved product with ID: {ProductId}", id);
        return Ok(result);
    }

    /// <summary>
    /// Gets all product catalog items.
    /// </summary>
    /// <returns>A list of product DTOs.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        logger.LogInformation("Attempting to retrieve all products.");
        var query = new GetAllProductsQuery();
        var result = await productService.GetAllProductsAsync(query);

        logger.LogInformation("Successfully retrieved {Count} products.", result.Count);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">The product creation request.</param>
    /// <returns>The newly created product DTO.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductCommand request)
    {
        logger.LogInformation("Attempting to create product: {ProductName}", request.Name);

        try
        {
            var newProduct = await productService.CreateProductAsync(request);
            logger.LogInformation("Successfully created product with ID: {ProductId}", newProduct.Id);
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Conflict during product creation: {Message}", ex.Message);
            return Conflict(ex.Message); // 409 Conflict if item already exists
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Validation error during product creation: {Message}", ex.Message);
            return BadRequest(ex.Errors); // FluentValidation errors
        }
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="request">The product update request.</param>
    /// <returns>The updated product DTO.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ProductDto>> UpdateProduct([FromBody] UpdateProductCommand request)
    {
        logger.LogInformation("Attempting to update product with ID: {ProductId}", request.Id);

        try
        {
            var updatedProduct = await productService.UpdateProductAsync(request);
            logger.LogInformation("Successfully updated product with ID: {ProductId}", updatedProduct.Id);
            return Ok(updatedProduct);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Not found during product update for ID {ProductId}.", request.Id);
            return NotFound(ex.Message); // 404 Not Found if item does not exist
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Validation error during product update: {Message}", ex.Message);
            return BadRequest(ex.Errors); // FluentValidation errors
        }
    }

    /// <summary>
    /// Deletes a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        logger.LogInformation("Attempting to delete product with ID: {ProductId}", id);
        var command = new DeleteProductCommand(id);

        try
        {
            await productService.DeleteProductAsync(command);
            logger.LogInformation("Successfully deleted product with ID: {ProductId}", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Not found during product deletion for ID {ProductId}.", id);
            return NotFound(ex.Message); // 404 Not Found if item does not exist
        }
    }
}
