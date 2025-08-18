using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Secretary.CreateSecretary;
using Educar.Backend.Application.Commands.Secretary.DeleteSecretary;
using Educar.Backend.Application.Commands.Secretary.UpdateSecretary;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Secretary;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Educar.Backend.Web.Endpoints;

public class Secretaries : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateSecretary)
            .MapGet(GetAllSecretaries)
            .MapGet(GetSecretary, "{id}")
            .MapPut(UpdateSecretary, "{id}")
            .MapDelete(DeleteSecretary, "{id}");
    }

    public async Task<IdResponseDto> CreateSecretary(ISender sender, CreateSecretaryCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<SecretaryDto> GetSecretary(ISender sender, Guid id)
    {
        return await sender.Send(new GetSecretaryQuery { Id = id });
    }

    public Task<PaginatedList<SecretaryDto>> GetAllSecretaries(
        ISender sender,
        [FromQuery(Name = "PageNumber")] int PageNumber,
        [FromQuery(Name = "PageSize")] int PageSize)
    {
        var query = new GetSecretariesPaginatedQuery
        {
            PageNumber = PageNumber,
            PageSize = PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteSecretary(ISender sender, Guid id)
    {
        await sender.Send(new DeleteSecretaryCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateSecretary(ISender sender, Guid id, UpdateSecretaryCommand command)
    {
        var updateCommand = new UpdateSecretaryCommand
        {
            Id = id,
            Name = command.Name,
            Description = command.Description,
            Code = command.Code,
            IsActive = command.IsActive
        };
        await sender.Send(updateCommand);
        return Results.NoContent();
    }
}
