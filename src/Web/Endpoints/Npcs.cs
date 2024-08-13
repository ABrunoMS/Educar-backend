using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Npc.CreateNpc;
using Educar.Backend.Application.Commands.Npc.DeleteNpc;
using Educar.Backend.Application.Commands.Npc.UpdateNpc;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Npc;
using Educar.Backend.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Npcs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPost(CreateNpc)
            .MapGet(GetNpc, "{id}")
            .MapGet(GetAllNpcByName)
            .MapPut(UpdateNpc, "{id}")
            .MapDelete(DeleteNpc, "{id}");
    }

    public Task<IdResponseDto> CreateNpc(ISender sender, CreateNpcCommand command)
    {
        return sender.Send(command);
    }

    public async Task<NpcDto> GetNpc(ISender sender, Guid id)
    {
        return await sender.Send(new GetNpcQuery { Id = id });
    }

    public Task<PaginatedList<NpcCleanDto>> GetAllNpcByName(ISender sender, [FromQuery(Name = "name")] string name,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetNpcsByNamePaginatedQuery(name)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteNpc(ISender sender, Guid id)
    {
        await sender.Send(new DeleteNpcCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateNpc(ISender sender, Guid id, UpdateNpcCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}