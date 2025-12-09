using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Quest.CreateQuest;
using Educar.Backend.Application.Commands.Quest.DeleteQuest;
using Educar.Backend.Application.Commands.Quest.UpdateQuest;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Quest;
using Educar.Backend.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Quests : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPost(CreateQuest)
            .MapGet(GetAllQuestsByGameGradeSubject)
            .MapPut(UpdateQuest, "{id}")
            .MapDelete(DeleteQuest, "{id}");

        app.MapGroup(this)
            .RequireAuthorization(UserRole.Student.GetDisplayName())
            .MapGet(GetQuest, "{id}");
    }

    public async Task<QuestDto> GetQuest(ISender sender, Guid id)
    {
        return await sender.Send(new GetQuestQuery { Id = id });
    }

    public Task<IdResponseDto> CreateQuest(ISender sender, CreateQuestCommand command)
    {
        return sender.Send(command);
    }

    public Task<PaginatedList<QuestCleanDto>> GetAllQuestsByGameGradeSubject(ISender sender,
        [FromQuery(Name = "game_id")] Guid? GameId,
        [FromQuery(Name = "grade_id")] Guid? GradeId,
        [FromQuery(Name = "subject_id")] Guid? SubjectId,
        [FromQuery(Name = "search")] string? Search,
        [FromQuery] bool UsageTemplate,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetQuestsByGameGradeSubjectPaginatedQuery
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize,
            GameId = GameId,
            GradeId = GradeId,
            SubjectId = SubjectId,
            Search = Search,
            UsageTemplate = UsageTemplate
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteQuest(ISender sender, Guid id)
    {
        await sender.Send(new DeleteQuestCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateQuest(ISender sender, Guid id, UpdateQuestCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}