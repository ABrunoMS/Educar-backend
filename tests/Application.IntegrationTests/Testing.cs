using Educar.Backend.Application.Commands.Client.CreateClient;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Infrastructure;
using Educar.Backend.Infrastructure.Data;
using Educar.Backend.Infrastructure.Data.Interceptors;
using Educar.Backend.Web;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Educar.Backend.Application.IntegrationTests;

[SetUpFixture]
public partial class Testing
{
    private static ServiceProvider ServiceProvider { get; set; } = null!;
    public static ApplicationDbContext Context { get; private set; } = null!;
    public static Mock<IObjectStorage> MockObjectStorage { get; private set; } = null!;
    public static Mock<IUser> MockCurrentUser { get; private set; } = null!;
    public static Mock<IIdentityService> MockIdentityServer { get; private set; } = null!;

    [OneTimeSetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();

        services.AddLogging(configure => configure.AddConsole());

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDatabase")
                .AddInterceptors(new SoftDeleteInterceptor());
        });

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables();

        // MockContext = new Mock<IApplicationDbContext>();
        services.AddApplicationServices();
        services.AddInfrastructureServices(builder.Build());
        services.AddWebServices();
        
        // Mock the IObjectStorage
        MockObjectStorage = new Mock<IObjectStorage>();
        services.AddSingleton(MockObjectStorage.Object);
        
        // Mock the IUser
        MockCurrentUser = new Mock<IUser>();
        services.AddSingleton(MockCurrentUser.Object);
        
        MockIdentityServer = new Mock<IIdentityService>();
        services.AddSingleton(MockIdentityServer.Object);

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
    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.Set<TEntity>().FindAsync(keyValues);
    }
    public static async Task<Guid> CreateClientAsAdminAsync(string clientName = "Cliente de Teste")
    {
        // Cria o comando completo com dados de teste
        var command = new CreateClientCommand
        {
            Name = clientName,
            Description = "Descrição de teste.",
            Partner = "Parceiro de Teste",
            Contacts = "Contato de Teste",
            Contract = "Contrato de Teste",
            Validity = "31/12/2099",
            SignatureDate = "08/08/2025",
            ImplantationDate = "08/08/2025",
            TotalAccounts = 1,
            Secretary = "Secretaria de Teste",
            SubSecretary = "Sub de Teste",
            Regional = "Regional de Teste"
        };
        
        // Envia o comando usando o método SendAsync que já existe
        var response = await SendAsync(command);

        // Retorna o ID do cliente criado
        return response.Id;
    }
}