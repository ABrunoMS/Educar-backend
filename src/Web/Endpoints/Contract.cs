using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Contract.CreateAccountType;
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
            .MapGet(GetContract, "{id}");
        // .MapPut(UpdateTodoList, "{id}")
        // .MapDelete(DeleteTodoList, "{id}");
    }

    public async Task<ContractDto> GetContract(ISender sender, Guid id)
    {
            return await sender.Send(new GetContractQuery { Id = id });
    }

    public Task<CreatedResponseDto> CreateContract(ISender sender, CreateContractCommand command)
    {
        return sender.Send(command);
    }
}