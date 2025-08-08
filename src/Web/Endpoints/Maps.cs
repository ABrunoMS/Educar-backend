using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Map.CreateMap;
using Educar.Backend.Application.Commands.Map.DeleteMap;
using Educar.Backend.Application.Commands.Map.UpdateMap;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Map;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Maps : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateMap)
            .MapGet(GetMap, "{id}")
            .MapGet(GetAllMapsByGame, "game/{gameId}")
            .MapPut(UpdateMap, "{id}")
            .MapDelete(DeleteMap, "{id}");
    }

    public Task<IdResponseDto> CreateMap(ISender sender, CreateMapCommand command)
    {
        return sender.Send(command);
    }

    public async Task<MapDto> GetMap(ISender sender, Guid id)
    {
        return await sender.Send(new GetMapQuery { Id = id });
    }

    public Task<PaginatedList<MapCleanDto>> GetAllMapsByGame(ISender sender, Guid gameId,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetMapByGamePaginatedQuery(gameId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteMap(ISender sender, Guid id)
    {
        await sender.Send(new DeleteMapCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateMap(ISender sender, Guid id, UpdateMapCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}