using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.QuestStepContent.CreateQuestStepContent;
using Educar.Backend.Application.Commands.QuestStepContent.DeleteQuestStepContent;
using Educar.Backend.Application.Commands.QuestStepContent.UpdateQuestStepContent;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.QuestStepContent;
using Educar.Backend.Application.Queries.School;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class QuestStepContents : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPost(CreateQuestStepContent)
            .MapGet(GetAllQuestStepContentByQuestStep, "queststep/{questStepId}")
            .MapGet(GetQuestStepContent, "{id}")
            .MapPut(UpdateQuestStepContent, "{id}")
            .MapDelete(DeleteQuestStepContent, "{id}");
    }

    public async Task<QuestStepContentDto> GetQuestStepContent(ISender sender, Guid id)
    {
        return await sender.Send(new GetQuestStepContentQuery() { Id = id });
    }

    public Task<CreatedResponseDto> CreateQuestStepContent(ISender sender, CreateQuestStepContentCommand command)
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

    public Task<PaginatedList<QuestStepContentDto>> GetAllQuestStepContentByQuestStep(ISender sender,
        Guid questStepId, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetQuestStepContentByQuestStepPaginatedQuery(questStepId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteQuestStepContent(ISender sender, Guid id)
    {
        await sender.Send(new DeleteQuestStepContentCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateQuestStepContent(ISender sender, Guid id, UpdateQuestStepContentCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}