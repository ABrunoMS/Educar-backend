using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Address.CreateAddress;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Addresses : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateAddress);
        // .MapGet(GetAccount, "{id}")
        // .MapGet(GetAllAccounts)
        // .MapPut(UpdateAccount, "{id}")
        // .MapDelete(DeleteAccount, "{id}");
    }

    public Task<CreatedResponseDto> CreateAddress(ISender sender, CreateAddressCommand command)
    {
        return sender.Send(command);
    }

    // public async Task<AccountDto> GetAccount(ISender sender, Guid id)
    // {
    //     return await sender.Send(new GetAccountQuery { Id = id });
    // }
    //
    // public Task<PaginatedList<AccountDto>> GetAllAccounts(ISender sender, [AsParameters] PaginatedQuery paginatedQuery)
    // {
    //     var query = new GetAccountsPaginatedQuery
    //     {
    //         PageNumber = paginatedQuery.PageNumber,
    //         PageSize = paginatedQuery.PageSize
    //     };
    //
    //     return sender.Send(query);
    // }
    //
    // public async Task<IResult> DeleteAccount(ISender sender, Guid id)
    // {
    //     await sender.Send(new DeleteAccountCommand(id));
    //     return Results.NoContent();
    // }
    //
    // public async Task<IResult> UpdateAccount(ISender sender, Guid id, UpdateAccountCommand command)
    // {
    //     command.Id = id;
    //     await sender.Send(command);
    //     return Results.NoContent();
    // }
}