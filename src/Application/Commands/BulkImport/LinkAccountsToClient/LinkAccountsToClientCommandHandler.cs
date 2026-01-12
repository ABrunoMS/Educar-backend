using Educar.Backend.Application.Commands.BulkImport;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Commands.BulkImport.LinkAccountsToClient;

public class LinkAccountsToClientCommandHandler(
    IApplicationDbContext context,
    ISpreadsheetService spreadsheetService)
    : IRequestHandler<LinkAccountsToClientCommand, LinkAccountsToClientResult>
{
    public async Task<LinkAccountsToClientResult> Handle(LinkAccountsToClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Ler e parsear a planilha
            List<AccountClientLinkRow> rows;
            try
            {
                rows = await spreadsheetService.ReadSpreadsheetAsync<AccountClientLinkRow>(request.FileStream, request.FileName);
            }
            catch (Exception ex)
            {
                return new LinkAccountsToClientResult
                {
                    Success = false,
                    ErrorMessage = $"Erro ao ler planilha: {ex.Message}"
                };
            }

            if (rows.Count == 0)
            {
                return new LinkAccountsToClientResult
                {
                    Success = false,
                    ErrorMessage = "A planilha não contém dados válidos."
                };
            }

            // 2. Validar TODAS as linhas ANTES de processar
            var validationResult = await ValidateAllRows(rows, cancellationToken);
            if (!string.IsNullOrEmpty(validationResult.ErrorMessage))
            {
                return validationResult;
            }

            // 3. Atualizar as contas
            try
            {
                var accountIds = rows.Select(r => r.AccountId).Distinct().ToList();
                
                // Busca todas as contas que serão atualizadas
                var accounts = await context.Accounts
                    .Where(a => accountIds.Contains(a.Id))
                    .ToListAsync(cancellationToken);

                int totalLinked = 0;
                int totalUpdated = 0;
                var warnings = new List<string>();

                foreach (var row in rows)
                {
                    var account = accounts.FirstOrDefault(a => a.Id == row.AccountId);
                    if (account == null)
                        continue; // Já foi validado, não deve acontecer

                    // Verifica se a conta já tinha um cliente diferente
                    if (account.ClientId.HasValue && account.ClientId.Value != row.ClientId)
                    {
                        var oldClientId = account.ClientId.Value;
                        warnings.Add($"Usuário '{account.Email}' (ID: {account.Id}) teve seu cliente alterado de {oldClientId} para {row.ClientId}");
                        totalUpdated++;
                    }
                    else if (!account.ClientId.HasValue)
                    {
                        totalLinked++;
                    }

                    // Atualiza o ClientId
                    account.ClientId = row.ClientId;
                }

                await context.SaveChangesAsync(cancellationToken);

                return new LinkAccountsToClientResult
                {
                    Success = true,
                    TotalLinked = totalLinked,
                    TotalUpdated = totalUpdated,
                    Warnings = warnings
                };
            }
            catch (Exception ex)
            {
                return new LinkAccountsToClientResult
                {
                    Success = false,
                    ErrorMessage = $"Erro ao vincular usuários no banco de dados: {ex.Message}"
                };
            }
        }
        catch (Exception ex)
        {
            return new LinkAccountsToClientResult
            {
                Success = false,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }

    private async Task<LinkAccountsToClientResult> ValidateAllRows(List<AccountClientLinkRow> rows, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // Valida campos vazios
        for (int i = 0; i < rows.Count; i++)
        {
            var lineNumber = i + 2; // +2 porque linha 1 é header e index começa em 0

            if (rows[i].AccountId == Guid.Empty)
                errors.Add($"Linha {lineNumber}: ID do usuário é obrigatório e deve ser um GUID válido.");

            if (rows[i].ClientId == Guid.Empty)
                errors.Add($"Linha {lineNumber}: ID do cliente é obrigatório e deve ser um GUID válido.");
        }

        // Se já há erros de formato, não precisa validar no banco
        if (errors.Any())
        {
            return new LinkAccountsToClientResult
            {
                Success = false,
                ErrorMessage = string.Join("; ", errors)
            };
        }

        // Verifica se TODAS as contas existem
        var accountIds = rows.Select(r => r.AccountId).Distinct().ToList();
        var existingAccounts = await context.Accounts
            .Where(a => accountIds.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        var missingAccounts = accountIds.Except(existingAccounts).ToList();
        if (missingAccounts.Any())
        {
            errors.Add($"Usuários não encontrados no sistema: {string.Join(", ", missingAccounts)}");
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

        if (errors.Any())
        {
            return new LinkAccountsToClientResult
            {
                Success = false,
                ErrorMessage = string.Join("; ", errors)
            };
        }

        return new LinkAccountsToClientResult { Success = true };
    }
}
