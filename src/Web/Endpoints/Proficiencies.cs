using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Proficiency.CreateProficiency;
using Educar.Backend.Application.Commands.Proficiency.DeleteProficiency;
using Educar.Backend.Application.Commands.Proficiency.UpdateProficiency;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Proficiency;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Proficiencies : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPost(CreateProficiency)
            .MapGet(GetAllProficiencies)
            .MapGet(GetProficiency, "{id}")
            .MapPut(UpdateProficiency, "{id}")
            .MapDelete(DeleteProficiency, "{id}");
    }

    public async Task<ProficiencyDto> GetProficiency(ISender sender, Guid id)
    {
        return await sender.Send(new GetProficiencyQuery { Id = id });
    }

    public Task<IdResponseDto> CreateProficiency(ISender sender, CreateProficiencyCommand command)
    {
        return sender.Send(command);
    }

    public Task<PaginatedList<ProficiencyCleanDto>> GetAllProficiencies(ISender sender,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetProficienciesPaginatedQuery
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteProficiency(ISender sender, Guid id)
    {
        await sender.Send(new DeleteProficiencyCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateProficiency(ISender sender, Guid id, UpdateProficiencyCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}