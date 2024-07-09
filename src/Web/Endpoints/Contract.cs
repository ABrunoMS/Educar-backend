using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Contract.CreateContract;
using Educar.Backend.Application.Commands.Contract.DeleteContract;
using Educar.Backend.Application.Commands.Contract.UpdateContract;
using Educar.Backend.Application.Queries.Contract;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Contract : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateContract)
            .MapGet(GetContract, "{id}")
            .MapPut(UpdateContract, "{id}")
            .MapDelete(DeleteContract, "{id}");
    }

    public async Task<ContractDto> GetContract(ISender sender, Guid id)
    {
        return await sender.Send(new GetContractQuery { Id = id });
    }

    public Task<CreatedResponseDto> CreateContract(ISender sender, CreateContractCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> DeleteContract(ISender sender, Guid id)
    {
        await sender.Send(new DeleteContractCommand(id));
        return Results.NoContent();
    }
    
    public async Task<IResult> UpdateContract(ISender sender, Guid id, UpdateContractCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}