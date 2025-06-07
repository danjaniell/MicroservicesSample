using InventoryService.Features.Inventories.Data;
using InventoryService.Features.Inventories.Services;

namespace InventoryService.Features.Startup;

/// <summary>
/// Extension methods for configuring inventory-related services.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds inventory specific services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    public static IServiceCollection AddInventoryServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IInventoryService, Inventories.Services.InventoryService>();

        return services;
    }
}
