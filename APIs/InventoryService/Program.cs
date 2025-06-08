using Asp.Versioning;
using Asp.Versioning.Builder;
using InventoryService.Extensions;
using InventoryService.Features.Startup;
using InventoryService.Middleware;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHealthChecks();

builder.Services.AddMemoryCacheWithFakes();

builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHostedService<MemoryCacheExtensions.CacheInitializationService>();

builder.Services.AddInventoryServices(builder.Configuration);
builder.Services.AddFluentValidationService();

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();

app.MapControllers();

app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

app.Run();