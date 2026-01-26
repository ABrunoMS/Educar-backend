using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Client.CreateClient;
using Educar.Backend.Application.Commands.Client.DeleteClient;
using Educar.Backend.Application.Commands.Client.UpdateClient;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Client;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Educar.Backend.Web.Endpoints;

public class Clients : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateClient) 
            .MapGet(GetAllClients)
            .MapPut(UpdateClient, "{id}")
            .MapDelete(DeleteClient, "{id}");
        app.MapGroup(this)
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Teacher" })
            .MapGet(GetClient, "{id}");
    }

    public Task<IdResponseDto> CreateClient(ISender sender, CreateClientCommand command)
    {
        return sender.Send(command);
    }

    public async Task<ClientDto> GetClient(ISender sender, Guid id)
    {
        return await sender.Send(new GetClientQuery { Id = id });
    }

    public Task<PaginatedList<ClientDto>> GetAllClients(
        ISender sender,
        [FromQuery(Name = "PageNumber")] int PageNumber,
        [FromQuery(Name = "PageSize")] int PageSize,
        [FromQuery(Name = "search")] string? Search)
    {
        var query = new GetClientsPaginatedQuery
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            Search = Search
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