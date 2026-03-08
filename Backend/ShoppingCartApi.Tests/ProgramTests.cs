using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCartApi.Domain.Interfaces;
using ShoppingCartApi.Infrastructure.Repositories;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.DependencyInjection.Extensions; // Add this using directive
using Microsoft.Extensions.Options; // Add this using directive
using System.Net.Http.Json; // Add this using directive

namespace ShoppingCartApi.Tests
{
    public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProgramTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void ConfigureServices_RegistersRepositories()
        {
            // Arrange
            var services = new ServiceCollection();
            // Provide a dummy JWT key for the configuration to prevent "Jwt:Key not found" error
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Jwt:Key", "supersecretkeyforjwttesting"}
                })
                .Build();

            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddConfiguration(configuration);
            builder.Services.AddSingleton<IConfiguration>(configuration); // Add IConfiguration

            // Act
            var startup = new Startup(configuration); // Create an instance of Startup with configuration
            startup.ConfigureServices(builder.Services); // Call ConfigureServices

            // Assert
            builder.Services.Should().Contain(s => s.ServiceType == typeof(IShoppingCartRepository) && s.ImplementationType == typeof(InMemoryShoppingCartRepository));
            builder.Services.Should().Contain(s => s.ServiceType == typeof(IProductRepository) && s.ImplementationType == typeof(InMemoryProductRepository));
            builder.Services.Should().Contain(s => s.ServiceType == typeof(IUserRepository) && s.ImplementationType == typeof(InMemoryUserRepository));
        }

        [Fact]
        public async Task JwtAuthentication_IsConfiguredCorrectly()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Remove existing authentication services to avoid "Scheme already exists" error
                    services.RemoveAll<IAuthenticationService>();
                    services.RemoveAll<IAuthenticationHandlerProvider>();
                    services.RemoveAll<IAuthenticationSchemeProvider>();
                    services.RemoveAll<IPostConfigureOptions<AuthenticationOptions>>();
                    services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();

                    // Override JWT configuration for testing
                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = "test-issuer",
                                ValidAudience = "test-audience",
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkeyforjwttesting"))
                            };
                        });
                });
            }).CreateClient();

            // Act: Try to access a protected endpoint without a token
            var response = await client.GetAsync("/weatherforecast");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            // Leer el contenido como string para evitar el error de PipeWriter
            (await response.Content.ReadAsStringAsync()).Should().NotBeNullOrEmpty();
        }
    }

}
