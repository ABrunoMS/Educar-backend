using Educar.Backend.Application.Commands.BulkImport;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Services;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Commands.BulkImport.ImportClasses;

public class ImportClassesCommandHandler(
    IApplicationDbContext context,
    ISpreadsheetService spreadsheetService)
    : IRequestHandler<ImportClassesCommand, ImportClassesResult>
{
    public async Task<ImportClassesResult> Handle(ImportClassesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Ler e parsear a planilha
            List<ClassImportRow> rows;
            try
            {
                rows = await spreadsheetService.ReadSpreadsheetAsync<ClassImportRow>(request.FileStream, request.FileName);
            }
            catch (Exception ex)
            {
                return new ImportClassesResult
                {
                    Success = false,
                    ErrorMessage = $"Erro ao ler planilha: {ex.Message}"
                };
            }

            if (rows.Count == 0)
            {
                return new ImportClassesResult
                {
                    Success = false,
                    ErrorMessage = "A planilha não contém dados válidos."
                };
            }

            // 2. Validar TODAS as linhas ANTES de inserir
            var validationErrors = await ValidateAllRows(rows, cancellationToken);
            if (validationErrors.Any())
            {
                return new ImportClassesResult
                {
                    Success = false,
                    ErrorMessage = string.Join("; ", validationErrors)
                };
            }

            // 3. Iniciar transação e inserir todas as turmas
            try
            {
                var classes = rows.Select(r => new Domain.Entities.Class(
                    name: r.ClassName.Trim(),
                    description: string.Empty,
                    purpose: ClassPurpose.Default)
                {
                    SchoolId = r.SchoolId,
                    IsActive = true
                }).ToList();

                await context.Classes.AddRangeAsync(classes, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                return new ImportClassesResult
                {
                    Success = true,
                    TotalInserted = classes.Count()
                };
            }
            catch (Exception ex)
            {
                return new ImportClassesResult
                {
                    Success = false,
                    ErrorMessage = $"Erro ao inserir turmas no banco de dados: {ex.Message}"
                };
            }
        }
        catch (Exception ex)
        {
            return new ImportClassesResult
            {
                Success = false,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }

    private async Task<List<string>> ValidateAllRows(List<ClassImportRow> rows, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // Valida campos vazios e formatos
        for (int i = 0; i < rows.Count; i++)
        {
            var lineNumber = i + 2; // +2 porque linha 1 é header e index começa em 0

            if (string.IsNullOrWhiteSpace(rows[i].ClassName))
                errors.Add($"Linha {lineNumber}: Nome da turma é obrigatório.");

            if (rows[i].SchoolId == Guid.Empty)
                errors.Add($"Linha {lineNumber}: ID da escola é obrigatório e deve ser um GUID válido.");
        }

        // Se já há erros de formato, não precisa validar no banco
        if (errors.Any())
            return errors;

        // Verifica se TODAS as escolas existem
        var schoolIds = rows.Select(r => r.SchoolId).Distinct().ToList();

        var existingSchools = await context.Schools
            .Where(s => schoolIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var missingSchools = schoolIds.Except(existingSchools).ToList();
        if (missingSchools.Any())
        {
            errors.Add($"Escolas não encontradas no sistema: {string.Join(", ", missingSchools)}");
        }

        return errors;
    }
}
