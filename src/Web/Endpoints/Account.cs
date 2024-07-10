using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Queries.Account;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Account : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateAccount)
            .MapGet(GetAccount, "{id}");
        // .MapPut(UpdateClient, "{id}")
        // .MapDelete(DeleteClient, "{id}");
    }

    public Task<CreatedResponseDto> CreateAccount(ISender sender, CreateAccountCommand command)
    {
        return sender.Send(command);
    }

    public async Task<AccountDto> GetAccount(ISender sender, Guid id)
    {
        return await sender.Send(new GetAccountQuery { Id = id });
    }
    //
    // public async Task<IResult> DeleteClient(ISender sender, Guid id)
    // {
    //     await sender.Send(new DeleteClientCommand(id));
    //     return Results.NoContent();
    // }
    //
    // public async Task<IResult> UpdateClient(ISender sender, Guid id, UpdateClientCommand command)
    // {
    //     command.Id = id;
    //     await sender.Send(command);
    //     return Results.NoContent();
    // }
}