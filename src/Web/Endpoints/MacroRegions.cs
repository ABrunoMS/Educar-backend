using Educar.Backend.Domain.Entities;
using Educar.Backend.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Educar.Backend.Web.Endpoints;

public class MacroRegions : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetAllMacroRegions);
    }

    public async Task<IEnumerable<object>> GetAllMacroRegions(ApplicationDbContext db)
    {
        var list = await db.MacroRegions.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
        return list.Select(m => new { id = m.Id, name = m.Name });
    }
}
