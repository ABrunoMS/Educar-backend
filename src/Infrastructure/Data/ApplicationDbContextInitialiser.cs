using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.Client.CreateClient;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Infrastructure.Options;
using Educar.Backend.Infrastructure.Data.Seeders;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ardalis.GuardClauses;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities; 

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
        // 0. Seed de BNCC a partir do CSV
        await SeedBnccAsync();

        // 1. Seed de Produtos e Conteúdos 
        await SeedProductsAndContentsAsync();

        // 1.1. Seed de Matérias e Séries (NOVO)
        await SeedSubjectsAndGradesAsync();

        // 2. Seed do Cliente Padrão (Secretaria)
        Guid? clientId = null;
        if (!await _context.Clients.AnyAsync())
        {
            var clientCommand = new CreateClientCommand
            {
                Name = "Educar",
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
                Regional = "Nacional",
                ProductIds = await _context.Products.Select(p => p.Id).ToListAsync(),
                ContentIds = await _context.Contents.Select(c => c.Id).ToListAsync()
            };
            
            var clientCreatedResponse = await _sender.Send(clientCommand, CancellationToken.None);
            clientId = clientCreatedResponse.Id;
            _logger.LogInformation("Cliente padrão 'Educar' criado.");
        }

        // 3. Seed do Usuário Admin
       /* if (!await _context.Accounts.AnyAsync(a => a.Role == UserRole.Admin))
        {
            
            if (clientId == null)
            {
                clientId = await _context.Clients.Select(c => c.Id).FirstOrDefaultAsync();
                Guard.Against.Default(clientId, nameof(clientId), "Nenhum cliente (secretaria) encontrado para associar o admin.");
            }

          var accountCommand = new CreateAccountCommand
            {
                Name = "admin-educar",
                LastName = "Educar", 
                Email = "admin@admin.com",
                RegistrationNumber = "000",
                ClientId = clientId.Value,
                Role = UserRole.Admin,
                Password = _options.DefaultAdminPassword,
                SchoolIds = new List<Guid>(), 
                ClassIds = new List<Guid>() 
            };
            await _sender.Send(accountCommand, CancellationToken.None);
            _logger.LogInformation("Usuário Administrador padrão criado.");
        }
        */
    }

    private async Task SeedProductsAndContentsAsync()
    {
        if (await _context.Products.AnyAsync())
        {
            return;
        }

        _logger.LogInformation("Iniciando o seed de Produtos e Conteúdos...");

        // 1. Criar as entidades de Conteúdo
        var cBNCC = new Content("BNCC");
        var cSAEB = new Content("SAEB");
        var cENEM = new Content("ENEM");
        var cJornadaDoTrabalho = new Content("JornadaDoTrabalho");
        var cEducacaoFinanceira = new Content("EducacaoFinanceira");
        var cEmpreendedorismo = new Content("Empreendedorismo");

        var allContents = new List<Content> { cBNCC, cSAEB, cENEM, cJornadaDoTrabalho, cEducacaoFinanceira, cEmpreendedorismo };
        await _context.Contents.AddRangeAsync(allContents);

        // 2. Criar as entidades de Produto
        var pOdisseiaE = new Product("OdisseiaEducacional");
        var pOdisseiaD = new Product("OdisseiaDungeons");
        var pJornadaSaber = new Product("JornadaSaber");
        var pTrilhaParaOFuturo = new Product("TrilhaParaOFuturo");
        var pJornadaDoTrabalho = new Product("JornadaDoTrabalho");
        var pRealidadeMagica = new Product("RealidadeMagica");

        // 3. Associar as Regras de Compatibilidade (criando as relações ProductContent)
        
        foreach(var content in allContents)
        {
            pOdisseiaE.ProductContents.Add(new ProductContent { Content = content });
            pOdisseiaD.ProductContents.Add(new ProductContent { Content = content });
        }

        pJornadaSaber.ProductContents.Add(new ProductContent { Content = cENEM });
        pJornadaSaber.ProductContents.Add(new ProductContent { Content = cSAEB });
        pTrilhaParaOFuturo.ProductContents.Add(new ProductContent { Content = cEducacaoFinanceira });
        pTrilhaParaOFuturo.ProductContents.Add(new ProductContent { Content = cEmpreendedorismo });
        pJornadaDoTrabalho.ProductContents.Add(new ProductContent { Content = cJornadaDoTrabalho });
        pRealidadeMagica.ProductContents.Add(new ProductContent { Content = cBNCC });

        await _context.Products.AddRangeAsync(pOdisseiaE, pOdisseiaD, pJornadaSaber, pTrilhaParaOFuturo, pJornadaDoTrabalho, pRealidadeMagica);
        
        
        await _context.SaveChangesAsync();
        _logger.LogInformation("Produtos, Conteúdos e Regras de Compatibilidade criados com sucesso.");
    }

     private async Task SeedSubjectsAndGradesAsync()
{
    // Verificação de Idempotência: Se já houver matérias ou séries, não faz nada.
    if (await _context.Subjects.AnyAsync() || await _context.Grades.AnyAsync())
    {
        return;
    }

    _logger.LogInformation("Iniciando o seed de Matérias (Subjects) e Séries (Grades)...");

    // 1. Criar lista de Matérias (Subjects)
    var subjects = new List<Subject>
    {
        new Subject("Arte", "Arte"),
        new Subject("Biologia", "Biologia"),
        new Subject("Ciências", "Ciências"),
        new Subject("Educação física", "Educação física"),
        new Subject("Ensino religioso", "Ensino religioso"),
        new Subject("Filosofia", "Filosofia"),
        new Subject("Física", "Física"),
        new Subject("Geografia", "Geografia"),
        new Subject("História", "História"),
        new Subject("Língua estrangeira", "Língua estrangeira"),
        new Subject("Língua Portuguesa", "Língua Portuguesa"),
        new Subject("Matemática", "Matemática"),
        new Subject("Química", "Química"),
        new Subject("Sociologia", "Sociologia")
    };

    await _context.Subjects.AddRangeAsync(subjects);

    // 2. Criar lista de Séries (Grades)
    var grades = new List<Grade>
    {
        new Grade("1° EF1", "1° EF1"),
        new Grade("2° EF1", "2° EF1"),
        new Grade("3° EF1", "3° EF1"),
        new Grade("4° EF1", "4° EF1"),
        new Grade("5° EF1", "5° EF1"),
        new Grade("6° EF2", "6° EF2"),
        new Grade("7° EF2", "7° EF2"),
        new Grade("8° EF2", "8° EF2"),
        new Grade("9° EF2", "9° EF2"),
        new Grade("1° EM", "1° EM"),
        new Grade("2° EM", "2° EM"),
        new Grade("3° EM", "3° EM")
    };

    await _context.Grades.AddRangeAsync(grades);

    // 3. Salvar no Banco
    await _context.SaveChangesAsync();
    
    _logger.LogInformation("Matérias e Séries criadas com sucesso (Nome e Descrição preenchidos).");
}


    private async Task SeedBnccAsync()
    {
        var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Seeds", "bncc.csv");
        
        
        var seeder = new BnccSeeder(_context, _logger);
        await seeder.SeedFromCsvAsync(csvPath);
    }
}