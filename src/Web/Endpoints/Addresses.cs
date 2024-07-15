using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Address.CreateAddress;
using Educar.Backend.Application.Commands.Address.DeleteAddress;
using Educar.Backend.Application.Commands.Address.UpdateAddress;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Addresses : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateAddress)
            .MapPut(UpdateAddress, "{id}")
            .MapDelete(DeleteAddress, "{id}");
    }

    public Task<CreatedResponseDto> CreateAddress(ISender sender, CreateAddressCommand command)
    {
        return sender.Send(command);
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