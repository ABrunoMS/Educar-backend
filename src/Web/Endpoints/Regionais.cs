using Educar.Backend.Application.Queries.Regional;
using Educar.Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Web.Endpoints;

public class Regionais : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetAllRegionais);
    }

    public async Task<IResult> GetAllRegionais(ApplicationDbContext db)
    {
        var regionais = await db.Regionais
            .Select(r => new RegionalDto
            {
                Id = r.Id,
                Name = r.Name,
                SubsecretariaId = r.SubsecretariaId
            })
            .ToListAsync();
        
        return Results.Ok(regionais);
    }
}
