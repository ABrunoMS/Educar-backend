using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.QuestStep.CreateQuestStep;
using Educar.Backend.Application.Commands.QuestStep.DeleteQuestStep;
using Educar.Backend.Application.Commands.QuestStep.UpdateQuestStep;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.QuestStep;
using Educar.Backend.Application.Queries.School;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class QuestSteps : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPost(CreateQuestStep)
            // .MapGet(GetAllQuestSteps);
            // .MapGet(GetAllSchoolsByClient, "client/{clientId}")
            .MapGet(GetQuestStep, "{id}")
            .MapPut(UpdateQuestStep, "{id}")
            .MapDelete(DeleteQuestStep, "{id}");
    }

    public async Task<QuestStepDto> GetQuestStep(ISender sender, Guid id)
    {
        return await sender.Send(new GetQuestStepQuery { Id = id });
    }

    public Task<IdResponseDto> CreateQuestStep(ISender sender, CreateQuestStepCommand command)
    {
        return sender.Send(command);
    }

    public Task<PaginatedList<SchoolDto>> GetAllSchools(ISender sender,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetSchoolsPaginatedQuery
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public Task<PaginatedList<SchoolDto>> GetAllSchoolsByClient(ISender sender,
        Guid clientId, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetSchoolsByClientPaginatedQuery(clientId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteQuestStep(ISender sender, Guid id)
    {
        await sender.Send(new DeleteQuestStepCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateQuestStep(ISender sender, Guid id, UpdateQuestStepCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}