using NpuApi.Repositories;
using NpuApi.Services;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using NpuApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddAWSService<IAmazonS3>();

if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));

    Console.WriteLine("Using SQLite database for local development");
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

    Console.WriteLine("Using AWS RDS PostgreSQL database");
}

builder.Services.AddScoped<ICreationRepository, CreationRepository>();
builder.Services.AddScoped<ICreationImageRepository, CreationImageRepository>();
builder.Services.AddScoped<ICreationScoreRepository, CreationScoreRepository>();

builder.Services.AddScoped<ICreationService, CreationService>();
builder.Services.AddScoped<ICreationScoreService, CreationScoreService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapPost("/creations", async (HttpRequest request, ICreationService creationService) =>
{
    try
    {
        if (!request.HasFormContentType)
        {
            return Results.BadRequest("Request must be multipart/form-data");
        }

        var form = await request.ReadFormAsync();

        var title = form["title"].ToString();
        var description = form["description"].ToString();
        var image = form.Files.GetFile("image");
        var nicePartName = form["nicePartName"].ToString();

        var isInvalidRequest = string.IsNullOrWhiteSpace(title) || image == null || string.IsNullOrWhiteSpace(nicePartName);
        if (isInvalidRequest)
        {
            return Results.BadRequest("Title, image and nicePartName are required");
        }

        var (id, imageUrl) = await creationService.CreateCreationAsync(title, description, nicePartName, image.OpenReadStream(), image.FileName);

        return Results.Created($"/creations/{id}", new { id, title, description, imageUrl, nicePartName });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error processing creation: {ex.Message}");
    }
});

app.MapPost("/creations/{id}/score", async (Guid id, HttpRequest request, ICreationScoreService creationScoreService) =>
{
    try
    {
        if (!request.HasFormContentType)
        {
            return Results.BadRequest("Request must be multipart/form-data");
        }

        var form = await request.ReadFormAsync();

        var uniqueness = form["uniqueness"].ToString();
        var creativity = form["creativity"].ToString();

        if (string.IsNullOrWhiteSpace(uniqueness) || string.IsNullOrWhiteSpace(creativity))
        {
            return Results.BadRequest("Uniqueness and creativity scores are required");
        }

        await creationScoreService.CreateCreationScoreAsync(id, int.Parse(uniqueness), int.Parse(creativity));

        return Results.Ok(new { message = "Score submitted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error processing score: {ex.Message}");
    }
});

app.MapGet("/creations", async (ICreationService creationService, string? search) =>
{
    try
    {
        var creations = await creationService.GetCreationsAsync(search);
        return Results.Ok(creations);
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error fetching creations: {ex.Message}");
    }
});

app.MapGet("/creations/{id}", async (Guid id, ICreationService creationService) =>
{
    try
    {
        var creation = await creationService.GetCreationByIdAsync(id);

        if (creation == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(creation);
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error fetching creation: {ex.Message}");
    }
});

app.Run();
