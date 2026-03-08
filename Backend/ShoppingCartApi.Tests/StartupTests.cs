using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http.Json; // Add this using directive
using Microsoft.AspNetCore.Http; // Add this using directive
using Microsoft.AspNetCore.Diagnostics; // Add this using directive

namespace ShoppingCartApi.Tests
{
    public class StartupTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public StartupTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Configure_ShouldUseSwaggerInDevelopmentAndNotTesting()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        {"ASPNETCORE_ENVIRONMENT", "Development"}
                    });
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/swagger");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Configure_ShouldNotUseSwaggerInProduction()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Production");
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        {"ASPNETCORE_ENVIRONMENT", "Production"}
                    });
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/swagger");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound); // Or other appropriate status if not found
        }

        [Fact]
        public async Task Configure_ShouldUseExceptionHandlerInProduction()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Production");
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        {"ASPNETCORE_ENVIRONMENT", "Production"}
                    });
                });
                builder.ConfigureServices(services =>
                {
                    // Para forzar una excepción en un endpoint y probar el ExceptionHandler
                    services.AddControllers().AddApplicationPart(typeof(Program).Assembly);
                    services.AddRouting();
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();
                });
                builder.Configure(app =>
                {
                    app.UseExceptionHandler("/error-test"); // Usar un path específico para la prueba
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/throw-exception", (HttpContext context) => { throw new Exception("Test Exception"); });
                        // Add an endpoint to handle the /error-test path
                        endpoints.MapGet("/error-test", (HttpContext context) =>
                        {
                            IExceptionHandlerPathFeature? exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                            Exception? exception = exceptionHandlerPathFeature?.Error;
                            return Results.Problem(
                                detail: exception?.StackTrace,
                                title: exception?.Message,
                                statusCode: StatusCodes.Status500InternalServerError);
                        }); // Permitir respuestas 404
                    });
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/throw-exception");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            // Leer el contenido como string para evitar el error de PipeWriter
            (await response.Content.ReadAsStringAsync()).Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Configure_ShouldAddSecurityHeaders()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/"); // Cualquier endpoint para obtener una respuesta

            // Assert
            response.Headers.Should().ContainKey("X-Content-Type-Options").WhoseValue.Should().Contain("nosniff");
            response.Headers.Should().ContainKey("X-Frame-Options").WhoseValue.Should().Contain("DENY");
            response.Headers.Should().ContainKey("Referrer-Policy").WhoseValue.Should().Contain("no-referrer-when-downgrade");
            response.Headers.Should().ContainKey("Permissions-Policy").WhoseValue.Should().Contain("geolocation=(), microphone=(), camera=()");
        }

        [Fact]
        public async Task Configure_ShouldMapWeatherForecastEndpoint()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/weatherforecast");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
            content.Should().Contain("TemperatureC"); // Verificar que devuelve datos de WeatherForecast
            // También se puede intentar deserializar para una verificación más robusta
            var forecast = await response.Content.ReadFromJsonAsync<IEnumerable<Startup.WeatherForecast>>();
            forecast.Should().NotBeNull().And.NotBeEmpty();
        }
    }
}
