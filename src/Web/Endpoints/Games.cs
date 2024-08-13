using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Game.CreateGame;
using Educar.Backend.Application.Commands.Game.DeleteGame;
using Educar.Backend.Application.Commands.Game.UpdateGame;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Game;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Games : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateGame)
            .MapGet(GetAllGames)
            .MapGet(GetGame, "{id}")
            .MapPut(UpdateGame, "{id}")
            .MapDelete(DeleteGame, "{id}");
    }

    public Task<IdResponseDto> CreateGame(ISender sender, CreateGameCommand command)
    {
        return sender.Send(command);
    }

    public async Task<GameDto> GetGame(ISender sender, Guid id)
    {
        return await sender.Send(new GetGameQuery { Id = id });
    }

    public Task<PaginatedList<GameDto>> GetAllGames(ISender sender, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetGamesPaginatedQuery
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteGame(ISender sender, Guid id)
    {
        await sender.Send(new DeleteGameCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateGame(ISender sender, Guid id, UpdateGameCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}