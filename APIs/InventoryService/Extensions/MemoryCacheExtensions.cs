using Bogus;
using InventoryService.Features.Inventories.Contracts;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace InventoryService.Extensions;

/// <summary>
/// Extension methods for IMemoryCache and fake data generation.
/// </summary>
public static class MemoryCacheExtensions
{
    private const string CacheKey = "InventoryItems";

    /// <summary>
    /// Adds IMemoryCache to the service collection and configures it with fake data initialization.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddMemoryCacheWithFakes(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IHostedService, CacheInitializationService>();
        return services;
    }

    /// <summary>
    /// Initializes the IMemoryCache with fake inventory data using Bogus.
    /// </summary>
    /// <param name="cache">The IMemoryCache instance.</param>
    /// <summary>
    /// Hosted service that initializes the cache with fake data when the application starts.
    /// </summary>
    public class CacheInitializationService(IMemoryCache cache, ILogger<CacheInitializationService> logger) : IHostedService
    {
        private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        private readonly ILogger<CacheInitializationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Initializing cache with fake data...");

                // Generate fake data
                var inventoryFaker = new Faker<Inventory>()
                    .CustomInstantiator(f => new Inventory(Guid.NewGuid(), f.Random.Number(0, 1000)))
                    .RuleFor(i => i.ProductId, (f, i) => i.ProductId)
                    .RuleFor(i => i.Quantity, (f, i) => i.Quantity);

                var items = new ConcurrentDictionary<Guid, Inventory>();
                for (int i = 0; i < 50; i++)
                {
                    var inventory = inventoryFaker.Generate();
                    if (!items.TryAdd(inventory.ProductId, inventory))
                    {
                        _logger.LogWarning("Duplicate ProductId generated: {ProductId}", inventory.ProductId);
                    }
                }

                // Set cache with new data
                _cache.Set(
                    CacheKey,
                    items,
                    new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }
                );

                _logger.LogInformation("Successfully initialized cache with {Count} inventory items", items.Count);

                // Verify the data was stored
                if (_cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Inventory>? cachedItems))
                {
                    _logger.LogDebug("Cache verification: {Count} items found in cache", cachedItems?.Count ?? 0);
                }
                else
                {
                    _logger.LogError("Failed to verify cache data after initialization");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing cache with fake data");
                throw;
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
