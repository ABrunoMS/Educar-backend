using Educar.Backend.Application.Queries.Subsecretaria;
using Educar.Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Web.Endpoints;

public class Subsecretarias : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetAllSubsecretarias);
    }

    public async Task<IResult> GetAllSubsecretarias(ApplicationDbContext db)
    {
        var subsecretarias = await db.Subsecretarias
            .Select(s => new SubsecretariaDto
            {
                Id = s.Id,
                Name = s.Name,
                ClientId = s.ClientId
            })
            .ToListAsync();
        
        return Results.Ok(subsecretarias);
    }
}
