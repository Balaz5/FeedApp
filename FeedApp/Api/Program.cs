using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            var app = builder.Build();

            await DataSeeder.SeedAsync(app.Services);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                    options.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Minimal health check endpoint
            app.MapGet("/", () => Results.Ok(new { Status = "Running", Timestamp = DateTime.UtcNow }))
                .WithName("HealthCheck")
                .WithTags("Health");

            app.Run();
        }
    }
}
