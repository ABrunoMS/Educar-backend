using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;
using Educar.Backend.Application.Commands.ProficiencyGroup.DeleteProficiencyGroup;
using Educar.Backend.Application.Commands.ProficiencyGroup.UpdateProficiencyGroup;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Proficiency;
using Educar.Backend.Application.Queries.ProficiencyGroup;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class ProficiencyGroups : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPost(CreateProficiencyGroup)
            .MapGet(GetAllProficiencyGroups)
            .MapGet(GetProficiencyGroup, "{id}")
            .MapPut(UpdateProficiencyGroup, "{id}")
            .MapDelete(DeleteProficiencyGroup, "{id}");
    }

    public async Task<ProficiencyGroupDto> GetProficiencyGroup(ISender sender, Guid id)
    {
        return await sender.Send(new GetProficiencyGroupQuery { Id = id });
    }

    public Task<IdResponseDto> CreateProficiencyGroup(ISender sender, CreateProficiencyGroupCommand command)
    {
        return sender.Send(command);
    }

    public Task<PaginatedList<ProficiencyGroupCleanDto>> GetAllProficiencyGroups(ISender sender,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetProficiencyGroupsPaginatedQuery
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteProficiencyGroup(ISender sender, Guid id)
    {
        await sender.Send(new DeleteProficiencyGroupCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateProficiencyGroup(ISender sender, Guid id, UpdateProficiencyGroupCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}