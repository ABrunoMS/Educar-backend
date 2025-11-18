using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Infrastructure.Data.Seeders;

public class BnccSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;

    public BnccSeeder(ApplicationDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedFromCsvAsync(string csvFilePath)
    {
        if (await _context.Bnccs.AnyAsync())
        {
            _logger.LogInformation("Tabela BNCC já contém dados. Seed ignorado.");
            return;
        }

        if (!File.Exists(csvFilePath))
        {
            _logger.LogWarning($"Arquivo CSV não encontrado: {csvFilePath}");
            return;
        }

        _logger.LogInformation($"Iniciando seed de BNCC a partir do arquivo: {csvFilePath}");

        try
        {
            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                MissingFieldFound = null,
                HeaderValidated = null
            });

            var records = csv.GetRecords<BnccCsvRecord>().ToList();
            var bnccs = records.Select(r => new Bncc(r.Description, r.IsActive)).ToList();

            await _context.Bnccs.AddRangeAsync(bnccs);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seed de BNCC concluído. {bnccs.Count} registros inseridos.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar o arquivo CSV de BNCC.");
            throw;
        }
    }
}

public class BnccCsvRecord
{
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
