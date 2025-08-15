using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Address.CreateAddress;
using Educar.Backend.Application.Commands.Address.DeleteAddress;
using Educar.Backend.Application.Commands.Address.UpdateAddress;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Address;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Educar.Backend.Web.Endpoints;

public class Addresses : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateAddress)
            .MapGet(GetAllAddresses)
            .MapGet(GetAddress, "{id}")
            .MapPut(UpdateAddress, "{id}")
            .MapDelete(DeleteAddress, "{id}");
    }

    public async Task<IdResponseDto> CreateAddress(ISender sender, CreateAddressCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<AddressDto> GetAddress(ISender sender, Guid id)
    {
        return await sender.Send(new GetAddressQuery { Id = id });
    }

    public Task<PaginatedList<AddressDto>> GetAllAddresses(
        ISender sender,
        [FromQuery(Name = "PageNumber")] int PageNumber,
        [FromQuery(Name = "PageSize")] int PageSize)
    {
        var query = new GetAddressesPaginatedQuery
        {
            PageNumber = PageNumber,
            PageSize = PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteAddress(ISender sender, Guid id)
    {
        await sender.Send(new DeleteAddressCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateAddress(ISender sender, Guid id, UpdateAddressCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}