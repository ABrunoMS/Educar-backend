using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.AccountType.CreateAccountType;
using Educar.Backend.Application.Commands.Client;
using Educar.Backend.Application.Commands.Client.UpdateClient;
using Educar.Backend.Application.Queries.Client;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Client : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateClient)
            .MapGet(GetClient, "{id}")
            .MapPut(UpdateClient, "{id}")
            .MapDelete(DeleteClient, "{id}");
    }

    public Task<CreatedResponseDto> CreateClient(ISender sender, CreateClientCommand command)
    {
        return sender.Send(command);
    }

    public async Task<ClientDto> GetClient(ISender sender, Guid id)
    {
        return await sender.Send(new GetClientQuery { Id = id });
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