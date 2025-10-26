using FlagshipAPI.Data;
using FlagshipAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CreditParameterService>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { ok = true }));

app.MapPost("/parameters", async (
    CreditParameter parameter,
    CreditParameterService service,
    AppDbContext db,
    ILogger<Program> logger) =>
{
    logger.LogInformation("POST /parameters - Product={Product}, Key={Key}", parameter.Product, parameter.Key);

    var hasOverlap = await service.HasOverlapAsync(parameter);
    if (hasOverlap)
    {
        logger.LogWarning(
            "Overlap detected for Product={Product}, Key={Key}, Start={Start}, End={End}",
            parameter.Product,
            parameter.Key,
            parameter.EffectiveStart,
            parameter.EffectiveEnd
        );

        return Results.BadRequest(new
        {
            message = "A parameter with this product, key, and overlapping validity period already exists."
        });
    }

    db.CreditParameters.Add(parameter);
    await db.SaveChangesAsync();

    logger.LogInformation(
        "Parameter created successfully. Id={Id}, Product={Product}, Key={Key}",
        parameter.Id,
        parameter.Product,
        parameter.Key
    );

    return Results.Created($"/parameters/{parameter.Id}", parameter);
});

app.MapGet("/parameters", async (string? product, string? key, AppDbContext db, ILogger<Program> logger) =>
{
    logger.LogInformation("GET /parameters - Filters: Product={Product}, Key={Key}", product, key);

    var query = db.CreditParameters.AsQueryable();

    if (!string.IsNullOrEmpty(product))
        query = query.Where(p => p.Product == product);

    if (!string.IsNullOrEmpty(key))
        query = query.Where(p => p.Key == key);

    var result = await query.ToListAsync();

    logger.LogInformation("GET /parameters - Returned {Count} records", result.Count);

    return Results.Ok(result);
});

app.Run();
