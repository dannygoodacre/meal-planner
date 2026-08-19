using System.Text.Json.Serialization;
using MealPlanner.Application;
using MealPlanner.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace MealPlanner.Web;

public sealed class Program
{
    public async static Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.AddData(builder.Configuration);

        builder.Services.AddApplication();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
            options.AddPolicy("Web", policy =>
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            )
        );

        WebApplication app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapFallbackToFile("index.html");

        using IServiceScope scope = app.Services.CreateScope();

        IServiceProvider services = scope.ServiceProvider;

        try
        {
            ApplicationContext context = services.GetRequiredService<ApplicationContext>();

            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while migrating: {ex.Message}");

            throw;
        }

        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseCors("Web");

        await app.RunAsync();
    }
}
