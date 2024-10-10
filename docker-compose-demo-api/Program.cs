using System.Reflection.Metadata.Ecma335;
using docker_compose_demo_api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<AppDbContext>();
await context.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/blogs", async () => await context.Blogs
    .Select(m => new GetBlogDto(m.Url, m.Posts
                .Select(p => new GetPostDto(p.PostId, p.Title, p.Content, m.BlogId)).ToList())
            ).ToListAsync())
.WithName("GetBlogs")
.WithOpenApi();

app.MapPost("/blogs", async (CreateBlogDto dto) =>
{
    var blog = new Blog()
    {
        Url = dto.Url
    };
    await context.Blogs.AddAsync(blog);
    await context.SaveChangesAsync();
    return Results.Created();
})
.WithName("CreateBlog")
.WithOpenApi();


app.MapPost("/posts", async (CreatePostDto dto) =>
{
    var post = new Post()
    {
        BlogId = dto.BlogId,
        Title = dto.Title,
        Content = dto.Content
    };
    await context.Posts.AddAsync(post);
    await context.SaveChangesAsync();
    return Results.Created();
})
.WithName("CreatePost")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record CreateBlogDto(string Url);
record GetBlogDto(string Url, List<GetPostDto> Posts);

record CreatePostDto(int BlogId, string Title, string Content);
record GetPostDto(int Id, string Title, string Content, int BlogId);
