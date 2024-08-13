using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Client.CreateClient;
using Educar.Backend.Application.Commands.Client.DeleteClient;
using Educar.Backend.Application.Commands.Client.UpdateClient;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Client;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Clients : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateClient)
            .MapGet(GetClient, "{id}")
            .MapGet(GetAllClients)
            .MapPut(UpdateClient, "{id}")
            .MapDelete(DeleteClient, "{id}");
    }

    public Task<IdResponseDto> CreateClient(ISender sender, CreateClientCommand command)
    {
        return sender.Send(command);
    }

    public async Task<ClientDto> GetClient(ISender sender, Guid id)
    {
        return await sender.Send(new GetClientQuery { Id = id });
    }

    public Task<PaginatedList<ClientDto>> GetAllClients(ISender sender, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetClientsPaginatedQuery
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteClient(ISender sender, Guid id)
    {
        await sender.Send(new DeleteClientCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateClient(ISender sender, Guid id, UpdateClientCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}