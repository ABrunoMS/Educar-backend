
using Educar.Backend.Application.Queries.Regional;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Educar.Backend.Web.Endpoints;

public static class Regionais
{
    public static void MapRegionalEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/regionais", async ([FromBody] RegionalDto dto, ApplicationDbContext db) =>
        {
            var entity = new Regional { Id = Guid.NewGuid(), Nome = dto.Nome, SubsecretariaId = dto.SubsecretariaId };
            db.Regionais.Add(entity);
            await db.SaveChangesAsync();
            return Results.Created($"/regionais/{entity.Id}", entity);
        });

        endpoints.MapGet("/regionais", async (ApplicationDbContext  db) =>
            await db.Regionais.ToListAsync());
    }
}
