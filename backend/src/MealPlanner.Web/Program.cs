using System.Text.Json.Serialization;
using MealPlanner.Application;
using MealPlanner.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace MealPlanner.Web;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddData(builder.Configuration);

        builder.Services.AddApplication();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddOpenApi();

        builder.Services.AddResponseCompression();

        builder.Services.Configure<StaticFileOptions>(options =>
            options.OnPrepareResponse = context =>
                context.Context.Response.Headers.CacheControl =
                    context.Context.Request.Path.StartsWithSegments("/assets")
                        ? "public, max-age=31536000, immutable"
                        : "no-cache"
        );

        WebApplication app = builder.Build();

        app.UseResponseCompression();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapFallbackToFile("index.html");

        using (IServiceScope scope = app.Services.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<ApplicationContext>().Database.MigrateAsync();
        }

        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.MapScalarApiReference();
        }

        await app.RunAsync();
    }
}
