using Microsoft.AspNetCore.Authentication.JwtBearer;
using MovieReservation.API.ErrorHandling;
using MovieReservation.API.Extensions;
using MovieReservation.API.OptionsSetup;
using MovieReservation.Application;
using MovieReservation.Application.Options;
using MovieReservation.Domain.Exceptions;
using MovieReservation.Infrastracture;
using MovieReservation.Infrastracture.Authentication;
using MovieReservation.Infrastracture.Persistence.Seeding;
using MovieReservation.Infrastracture.Serialization.Converters;
using Serilog;

namespace MovieReservation.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter(builder.Configuration["DateOnly:Format"]!));
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            };
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services
            .AddApplication()
            .AddInfrastracture();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearerConfiguration(builder.Configuration);

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
        builder.Services.Configure<AdminOptions>(builder.Configuration.GetSection("Admin"));
        builder.Services.ConfigureOptions<DateOnlyOptionsSetup>();

        var app = builder.Build();

        if (!app.Environment.IsStaging())
        {
            using var scope = app.Services.CreateScope();
            try
            {
                await DbInitializer.SeedData(scope.ServiceProvider);
            }
            catch (Exception ex) when (ex is SeedingException)
            {
                Log.ForContext(typeof(DbInitializer)).Fatal(ex, "Seeding failed");
            }
        }
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}