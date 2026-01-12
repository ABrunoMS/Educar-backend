using Educar.Backend.Application.Commands.BulkImport;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Services;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Commands.BulkImport.ImportAccounts;

public class ImportAccountsCommandHandler(
    IApplicationDbContext context,
    ISpreadsheetService spreadsheetService)
    : IRequestHandler<ImportAccountsCommand, ImportAccountsResult>
{
    public async Task<ImportAccountsResult> Handle(ImportAccountsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Ler e parsear a planilha
            List<AccountImportRow> rows;
            try
            {
                rows = await spreadsheetService.ReadSpreadsheetAsync<AccountImportRow>(request.FileStream, request.FileName);
            }
            catch (Exception ex)
            {
                return new ImportAccountsResult
                {
                    Success = false,
                    ErrorMessage = $"Erro ao ler planilha: {ex.Message}"
                };
            }

            if (rows.Count == 0)
            {
                return new ImportAccountsResult
                {
                    Success = false,
                    ErrorMessage = "A planilha não contém dados válidos."
                };
            }

            // 2. Validar TODAS as linhas ANTES de inserir
            var validationErrors = await ValidateAllRows(rows, cancellationToken);
            if (validationErrors.Any())
            {
                return new ImportAccountsResult
                {
                    Success = false,
                    ErrorMessage = string.Join("; ", validationErrors)
                };
            }

            // 3. Inserir todas as contas
            try
            {
                var accounts = rows.Select(r => new Domain.Entities.Account(
                    name: r.Name.Trim(),
                    email: r.Email.Trim().ToLower(),
                    role: UserRole.Student) // Padrão: estudantes
                {
                    ClientId = r.ClientId,
                    Password = r.Password,
                    LastName = r.LastName.Trim(),
                    RegistrationNumber = null,
                    AverageScore = 0,
                    EventAverageScore = 0,
                    Stars = 0
                }).ToList();

                // Adiciona o evento de criação para cada conta
                foreach (var account in accounts)
                {
                    account.AddDomainEvent(new AccountCreatedEvent(account));
                }

                await context.Accounts.AddRangeAsync(accounts, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                return new ImportAccountsResult
                {
                    Success = true,
                    TotalInserted = accounts.Count()
                };
            }
            catch (Exception ex)
            {
                return new ImportAccountsResult
                {
                    Success = false,
                    ErrorMessage = $"Erro ao inserir usuários no banco de dados: {ex.Message}"
                };
            }
        }
        catch (Exception ex)
        {
            return new ImportAccountsResult
            {
                Success = false,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }

    private async Task<List<string>> ValidateAllRows(List<AccountImportRow> rows, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // Valida campos vazios e formatos
        for (int i = 0; i < rows.Count; i++)
        {
            var lineNumber = i + 2; // +2 porque linha 1 é header e index começa em 0

            if (string.IsNullOrWhiteSpace(rows[i].Name))
                errors.Add($"Linha {lineNumber}: Nome do usuário é obrigatório.");

            if (string.IsNullOrWhiteSpace(rows[i].LastName))
                errors.Add($"Linha {lineNumber}: Sobrenome do usuário é obrigatório.");

            if (string.IsNullOrWhiteSpace(rows[i].Email))
                errors.Add($"Linha {lineNumber}: Email é obrigatório.");
            else if (!IsValidEmail(rows[i].Email))
                errors.Add($"Linha {lineNumber}: Email '{rows[i].Email}' é inválido.");

            if (string.IsNullOrWhiteSpace(rows[i].Password))
                errors.Add($"Linha {lineNumber}: Senha é obrigatória.");
            else if (rows[i].Password.Length < 6)
                errors.Add($"Linha {lineNumber}: Senha deve ter no mínimo 6 caracteres.");

            if (rows[i].ClientId == Guid.Empty)
                errors.Add($"Linha {lineNumber}: ID do cliente é obrigatório e deve ser um GUID válido.");
        }

        // Se já há erros de formato, não precisa validar no banco
        if (errors.Any())
            return errors;

        // Verifica se há emails duplicados na planilha
        var duplicateEmails = rows
            .GroupBy(r => r.Email.ToLower())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateEmails.Any())
        {
            errors.Add($"Emails duplicados na planilha: {string.Join(", ", duplicateEmails)}");
        }

        // Verifica se os emails já existem no banco
        var emails = rows.Select(r => r.Email.ToLower()).Distinct().ToList();
        var existingEmails = await context.Accounts
            .Where(a => emails.Contains(a.Email.ToLower()))
            .Select(a => a.Email)
            .ToListAsync(cancellationToken);

        if (existingEmails.Any())
        {
            errors.Add($"Emails já cadastrados no sistema: {string.Join(", ", existingEmails)}");
        }

        // Verifica se TODOS os clientes existem
        var clientIds = rows.Select(r => r.ClientId).Distinct().ToList();

        var existingClients = await context.Clients
            .Where(c => clientIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var missingClients = clientIds.Except(existingClients).ToList();
        if (missingClients.Any())
        {
            errors.Add($"Clientes não encontrados no sistema: {string.Join(", ", missingClients)}");
        }

        return errors;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
