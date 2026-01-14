using Educar.Backend.Application.Commands.BulkImport;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Educar.Backend.Application.Commands.BulkImport.LinkQuestsToClasses;

public class LinkQuestsToClassesCommandHandler(
    IApplicationDbContext context,
    ISpreadsheetService spreadsheetService)
    : IRequestHandler<LinkQuestsToClassesCommand, LinkQuestsToClassesResult>
{
    public async Task<LinkQuestsToClassesResult> Handle(LinkQuestsToClassesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Ler e parsear a planilha
            List<QuestClassLinkRow> rows;
            try
            {
                rows = await spreadsheetService.ReadSpreadsheetAsync<QuestClassLinkRow>(request.FileStream, request.FileName);
            }
            catch (Exception ex)
            {
                return new LinkQuestsToClassesResult
                {
                    Success = false,
                    ErrorMessage = $"Erro ao ler planilha: {ex.Message}"
                };
            }

            if (rows.Count == 0)
            {
                return new LinkQuestsToClassesResult
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

            // 3. Processar as vinculações
            try
            {
                int totalLinked = 0;
                int totalUpdated = 0;
                var warnings = new List<string>();

                foreach (var row in rows)
                {
                    // Parse da data em formato ISO ou DD/MM/YYYY
                    DateTimeOffset expirationDateOffset;
                    
                    if (DateTimeOffset.TryParse(row.ExpirationDate, out var parsedDateOffset))
                    {
                        // Formato ISO - converter para UTC
                        expirationDateOffset = parsedDateOffset.ToUniversalTime();
                    }
                    else if (DateTime.TryParseExact(
                        row.ExpirationDate,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var expirationDate))
                    {
                        // Formato DD/MM/YYYY - criar como UTC
                        expirationDateOffset = new DateTimeOffset(expirationDate, TimeSpan.Zero);
                    }
                    else
                    {
                        // Já foi validado, não deve acontecer
                        continue;
                    }

                    // Verifica se já existe a vinculação
                    var existingClassQuest = await context.ClassQuests
                        .FirstOrDefaultAsync(cq => cq.QuestId == row.QuestId && cq.ClassId == row.ClassId, cancellationToken);

                    if (existingClassQuest != null)
                    {
                        // Atualizar a data de expiração
                        var oldDate = existingClassQuest.ExpirationDate;
                        existingClassQuest.ExpirationDate = expirationDateOffset;
                        
                        warnings.Add($"Aula ID {row.QuestId} já estava na turma ID {row.ClassId}. Data de expiração alterada de {oldDate:dd/MM/yyyy} para {expirationDateOffset:dd/MM/yyyy}");
                        totalUpdated++;
                    }
                    else
                    {
                        // Criar nova vinculação
                        var classQuest = new Domain.Entities.ClassQuest
                        {
                            QuestId = row.QuestId,
                            ClassId = row.ClassId,
                            ExpirationDate = expirationDateOffset
                        };

                        context.ClassQuests.Add(classQuest);
                        totalLinked++;
                    }
                }

                await context.SaveChangesAsync(cancellationToken);

                return new LinkQuestsToClassesResult
                {
                    Success = true,
                    TotalLinked = totalLinked,
                    TotalUpdated = totalUpdated,
                    Warnings = warnings
                };
            }
            catch (Exception ex)
            {
                return new LinkQuestsToClassesResult
                {
                    Success = false,
                    ErrorMessage = $"Erro ao vincular aulas no banco de dados: {ex.Message}"
                };
            }
        }
        catch (Exception ex)
        {
            return new LinkQuestsToClassesResult
            {
                Success = false,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }

    private async Task<LinkQuestsToClassesResult> ValidateAllRows(List<QuestClassLinkRow> rows, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // Valida campos vazios e formatos
        for (int i = 0; i < rows.Count; i++)
        {
            var lineNumber = i + 2; // +2 porque linha 1 é header e index começa em 0

            if (rows[i].QuestId == Guid.Empty)
                errors.Add($"Linha {lineNumber}: ID da aula é obrigatório e deve ser um GUID válido.");

            if (rows[i].ClassId == Guid.Empty)
                errors.Add($"Linha {lineNumber}: ID da turma é obrigatório e deve ser um GUID válido.");

            if (string.IsNullOrWhiteSpace(rows[i].ExpirationDate))
            {
                errors.Add($"Linha {lineNumber}: Data de expiração é obrigatória.");
            }
            else
            {
                // Validar formato da data
                bool isValidDate = false;
                
                if (DateTimeOffset.TryParse(rows[i].ExpirationDate, out _))
                {
                    isValidDate = true;
                }
                else if (DateTime.TryParseExact(
                    rows[i].ExpirationDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _))
                {
                    isValidDate = true;
                }

                if (!isValidDate)
                {
                    errors.Add($"Linha {lineNumber}: Data de expiração inválida. Use o formato ISO (YYYY-MM-DDTHH:mm:ssZ) ou DD/MM/YYYY.");
                }
            }
        }

        // Se já há erros de formato, não precisa validar no banco
        if (errors.Any())
        {
            return new LinkQuestsToClassesResult
            {
                Success = false,
                ErrorMessage = string.Join("; ", errors)
            };
        }

        // Verifica se TODAS as aulas (quests) existem
        var questIds = rows.Select(r => r.QuestId).Distinct().ToList();
        var existingQuests = await context.Quests
            .Where(q => questIds.Contains(q.Id))
            .Select(q => q.Id)
            .ToListAsync(cancellationToken);

        var missingQuests = questIds.Except(existingQuests).ToList();
        if (missingQuests.Any())
        {
            errors.Add($"Aulas não encontradas no sistema: {string.Join(", ", missingQuests)}");
        }

        // Verifica se TODAS as turmas (classes) existem
        var classIds = rows.Select(r => r.ClassId).Distinct().ToList();
        var existingClasses = await context.Classes
            .Where(c => classIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var missingClasses = classIds.Except(existingClasses).ToList();
        if (missingClasses.Any())
        {
            errors.Add($"Turmas não encontradas no sistema: {string.Join(", ", missingClasses)}");
        }

        if (errors.Any())
        {
            return new LinkQuestsToClassesResult
            {
                Success = false,
                ErrorMessage = string.Join("; ", errors)
            };
        }

        return new LinkQuestsToClassesResult { Success = true };
    }
}
