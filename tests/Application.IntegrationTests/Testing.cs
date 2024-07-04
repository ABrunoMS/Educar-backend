using Educar.Backend.Application.Interfaces;
using Educar.Backend.Infrastructure;
using Educar.Backend.Infrastructure.Data;
using Educar.Backend.Web;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Educar.Backend.Application.IntegrationTests;

[SetUpFixture]
public class Testing
{
    private static ServiceProvider ServiceProvider { get; set; } = null!;
    public static ApplicationDbContext Context { get; private set; } = null!;
    
    [OneTimeSetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();

        services.AddLogging(configure => configure.AddConsole());

        services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase("TestDatabase"); });

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables();

        // MockContext = new Mock<IApplicationDbContext>();
        services.AddApplicationServices();
        services.AddInfrastructureServices(builder.Build());
        services.AddWebServices();

        ServiceProvider = services.BuildServiceProvider();
        Context = ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        ServiceProvider.Dispose();
        Context.Dispose();
    }

    public static void ResetState()
    {
        using var scope = ServiceProvider.CreateScope();
        Context.Database.EnsureDeleted();
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await mediator.Send(request);
    }
}