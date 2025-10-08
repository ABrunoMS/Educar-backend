using Educar.Backend.Application.Queries.Subsecretaria;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Web.Endpoints;

public static class Subsecretarias
{
    public static void MapSubsecretariaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/subsecretarias", async ([FromBody] SubsecretariaDto dto, ApplicationDbContext db) =>
        {
            var entity = new Subsecretaria { Id = Guid.NewGuid(), Nome = dto.Nome };
            db.Subsecretarias.Add(entity);
            await db.SaveChangesAsync();
            return Results.Created($"/subsecretarias/{entity.Id}", entity);
        });

        endpoints.MapGet("/subsecretarias", async (ApplicationDbContext db) =>
            await db.Subsecretarias.ToListAsync());
    }
}
