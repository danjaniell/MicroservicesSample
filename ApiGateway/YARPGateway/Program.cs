using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Polly; 
using System.Net;
using Microsoft.Extensions.Http.Resilience; 

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"yarp.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromSeconds(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0 
            }));

    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromMinutes(1);
        options.PermitLimit = 10;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("API rate limit exceeded. Please try again later.", token);
    };
});

builder.Services.AddHttpClient("DownstreamClient")
    .AddResilienceHandler("default", pipelineBuilder =>
    {
        pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(2),
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            ShouldHandle = static args =>
            {
                if (args.Outcome.Result is HttpResponseMessage response)
                {
                    if ((int)response.StatusCode >= 500 || response.StatusCode == HttpStatusCode.RequestTimeout)
                    {
                        return new ValueTask<bool>(true);
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return new ValueTask<bool>(true);
                    }
                }
                if (args.Outcome.Exception is HttpRequestException)
                {
                    return new ValueTask<bool>(true);
                }
                return new ValueTask<bool>(false);
            }
        });

        pipelineBuilder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = 0.1,
            SamplingDuration = TimeSpan.FromSeconds(10),
            MinimumThroughput = 20,
            BreakDuration = TimeSpan.FromSeconds(5),
            ShouldHandle = static args =>
            {
                if (args.Outcome.Result is HttpResponseMessage response)
                {
                    if ((int)response.StatusCode >= 500 || response.StatusCode == HttpStatusCode.RequestTimeout)
                    {
                        return new ValueTask<bool>(true);
                    }
                }
                if (args.Outcome.Exception is HttpRequestException)
                {
                    return new ValueTask<bool>(true);
                }
                return new ValueTask<bool>(false);
            }
        });

        pipelineBuilder.AddTimeout(TimeSpan.FromSeconds(15));
    });


builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("longCache", builder => builder.Expire(TimeSpan.FromSeconds(300)));
    options.AddPolicy("shortCache", builder => builder.Expire(TimeSpan.FromSeconds(60)));
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRateLimiter(); 

app.UseOutputCache();

app.MapReverseProxy();

app.Run();