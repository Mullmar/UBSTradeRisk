using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using TradeRisk.Application.Dtos;
using TradeRisk.Application.Services;
using TradeRisk.Domain.Classification;
using TradeRisk.Domain.Classification.Interfaces;
using TradeRisk.Domain.Classification.Rules;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

// Logging básico
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// DI — registrar regras e serviços
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .Select(e => new { field = e.Key, errors = e.Value!.Errors.Select(x => x.ErrorMessage) });
            return new BadRequestObjectResult(new { message = "Validation failed", errors });
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "TradeRisk API";
    config.Version = "v1";
});



// Regras de risco (ordem é importante para prioridade)
builder.Services.AddSingleton<IRiskRule, LowRiskRule>();
builder.Services.AddSingleton<IRiskRule, MediumRiskRule>();
builder.Services.AddSingleton<IRiskRule, HighRiskRule>();

// Classifier e Service
builder.Services.AddSingleton<TradeRiskClassifier>(sp =>
{
    var rules = sp.GetServices<IRiskRule>(); // mantém a ordem de registro
    return new TradeRiskClassifier(rules);
});
builder.Services.AddScoped<ITradeRiskService, TradeRiskService>();

var app = builder.Build();

// Habilita Swagger
app.UseOpenApi();    // gera /swagger/v1/swagger.json
app.UseSwaggerUi(); // interface Swagger UI

var urls = builder.WebHost.GetSetting("urls")?.Split(';') ?? Array.Empty<string>();
foreach (var url in urls)
{
    Console.WriteLine($"Aplicação rodando em: {url}/swagger");
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            var result = new
            {
                message = "Ocorreu um erro inesperado",
                detail = error.Error.Message
            };
            await context.Response.WriteAsJsonAsync(result);
        }
    });
});

app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { } // para testes de integração
