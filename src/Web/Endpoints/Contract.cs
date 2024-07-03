using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Contract.CreateAccountType;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Contract : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateContract);
        // .MapGet(GetTodoLists)
        // .MapPut(UpdateTodoList, "{id}")
        // .MapDelete(DeleteTodoList, "{id}");
    }

    public Task<CreatedResponseDto> CreateContract(ISender sender, CreateContractCommand command)
    {
        return sender.Send(command);
    }
}