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
using Ardalis.GuardClauses; // Adicionado para o Guard.Against.Null

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
            // CÓDIGO ATUALIZADO AQUI
            // Criamos o comando preenchendo todos os campos obrigatórios.
            var clientCommand = new CreateClientCommand
            {
                Name = clientName,
                Description = "Cliente principal do sistema, criado na inicialização.",
                Partner = "Educar Padrão",
                Contacts = "admin@educar.com",
                Contract = "Contrato Master",
                Validity = "31/12/2099",
                SignatureDate = "07/08/2025",
                ImplantationDate = "07/08/2025",
                TotalAccounts = 999,
                Secretary = "Secretaria Principal",
                SubSecretary = "N/A",
                Regional = "Nacional"
            };
            
            var clientCreatedResponse = await _sender.Send(clientCommand, CancellationToken.None);
            clientId = clientCreatedResponse.Id;
        }

        // If there are no accounts, create an admin account
        if (!_context.Accounts.Any())
        {
            Guard.Against.Null(clientId, nameof(clientId));
            var accountCommand = new CreateAccountCommand
            {
                Name = "admin-educar",
                Email = "admin@admin.com",
                RegistrationNumber = "000",
                ClientId = clientId.Value,
                Role = UserRole.Admin,
                Password = _options.DefaultAdminPassword 
            };
            await _sender.Send(accountCommand, CancellationToken.None);
        }
    }
}