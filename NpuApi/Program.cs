using NpuApi.Repositories;
using NpuApi.Services;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using NpuApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
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

        if (string.IsNullOrWhiteSpace(title) || image == null)
        {
            return Results.BadRequest("Title and image are required");
        }

        var (id, imageUrl) = await creationService.CreateCreationAsync(title, description, image.OpenReadStream(), image.FileName);

        return Results.Created($"/creations/{id}", new { id, title, description, imageUrl });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error processing creation: {ex.Message}");
    }
});

app.MapPost("/creations/{id}/score", async (string id, HttpRequest request, ICreationRepository creationRepository) =>
{
    try
    {
        if (!request.HasFormContentType)
        {
            return Results.BadRequest("Request must be multipart/form-data");
        }

        var form = await request.ReadFormAsync();
        
        var score = form["score"].ToString();

        if (string.IsNullOrWhiteSpace(score))
        {
            return Results.BadRequest("Score is required");
        }

        

        return Results.Ok(new { message = "Score submitted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error processing score: {ex.Message}");
    }
});

app.MapGet("/creations", async (ICreationService creationService, string? search) =>
{
    var creations = await creationService.GetCreationsAsync(search);
    return Results.Ok(creations);
});

app.MapGet("/creations/{id}", async (Guid id, ICreationService creationService) =>
{
    var creation = await creationService.GetCreationByIdAsync(id);
    
    if (creation == null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(creation);
});

app.Run();
