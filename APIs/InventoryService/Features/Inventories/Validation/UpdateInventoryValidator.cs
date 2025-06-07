using FluentValidation;
using InventoryService.Features.Inventories.Commands;

namespace InventoryService.Features.Inventories.Validation;

/// <summary>
/// Validator for the <see cref="UpdateInventoryCommand"/>.
/// </summary>
public sealed class UpdateInventoryValidator : AbstractValidator<UpdateInventoryCommand>
{
    public UpdateInventoryValidator()
    {
        RuleFor(command => command.ProductId)
            .NotEmpty().WithMessage("Product ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Product ID cannot be an empty GUID.");

        RuleFor(command => command.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be a non-negative value.");
    }
}
