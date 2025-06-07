using ProductCatalogService.Features.Products.Data;
using ProductCatalogService.Features.Products.Services;

namespace ProductCatalogService.Features.Startup;

/// <summary>
/// Extension methods for configuring product catalog-related services.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds product catalog specific services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    public static IServiceCollection AddProductCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
