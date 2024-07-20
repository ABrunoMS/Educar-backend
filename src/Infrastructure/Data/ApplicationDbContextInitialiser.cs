using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.Client.CreateClient;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ISender _sender;
    private readonly InitDataOptions _options;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context, ISender sender, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _sender = sender;
        _options = configuration.GetInitDataOptions();
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            if (_options.Active)
            {
                await TrySeedAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        Guid? clientId = null;
        var clientName = "Educar";

        // Check if there are any clients, if not, create one
        if (!_context.Clients.Any())
        {
            var clientCommand = new CreateClientCommand(clientName);
            var clientCreatedResponse = await _sender.Send(clientCommand, CancellationToken.None);
            clientId = clientCreatedResponse.Id;
        }

        // If there are no accounts, create an admin account
        if (!_context.Accounts.Any())
        {
            Guard.Against.Null(clientId, nameof(clientId));

            var accountCommand = new CreateAccountCommand("admin-educar", "admin@admin.com", "000", clientId.Value,
                UserRole.Admin);
            await _sender.Send(accountCommand, CancellationToken.None);
        }
    }
}