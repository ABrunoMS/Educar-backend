using Educar.Backend.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Infrastructure.Data.Seeders;

public class MacroRegionSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;

    public MacroRegionSeeder(ApplicationDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        // Ensure table exists (safe when migrations not present) and insert standard macro regions
        var createTableSql = @"
        CREATE TABLE IF NOT EXISTS macro_regions (
            id uuid PRIMARY KEY,
            name text NOT NULL,
            is_deleted boolean DEFAULT false,
            deleted_at timestamp with time zone
        );
        ";

        await _context.Database.ExecuteSqlRawAsync(createTableSql);

        var inserts = new[] { "Norte", "Nordeste", "Centro-Oeste", "Sudeste", "Sul" };
        foreach (var name in inserts)
        {
            var insertSql = @"INSERT INTO macro_regions (id, name) SELECT @p0::uuid, @p1 WHERE NOT EXISTS (SELECT 1 FROM macro_regions WHERE name = @p1);";
            await _context.Database.ExecuteSqlRawAsync(insertSql, Guid.NewGuid(), name);
        }

        _logger.LogInformation("MacroRegions seeded.");
    }
}
