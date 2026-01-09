using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.ClassQuest.CreateClassQuest;
using Educar.Backend.Application.Commands.ClassQuest.UpdateClassQuest;
using Educar.Backend.Application.Commands.ClassQuest.DeleteClassQuest;
using Educar.Backend.Application.Queries.ClassQuest;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Web.Endpoints;

public record UpdateClassQuestRequest(string ExpirationDate);

public class ClassQuests : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.ToString())
            .MapPost(CreateClassQuest)
            .MapPut(UpdateClassQuest, "{id}")
            .MapDelete(DeleteClassQuest, "{id}")
            .MapGet(GetClassQuestById, "{id}")
            .MapGet(GetClassQuests);
    }

    public Task<IdResponseDto> CreateClassQuest(ISender sender, CreateClassQuestCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdateClassQuest(ISender sender, Guid id, UpdateClassQuestRequest request)
    {
        var command = new UpdateClassQuestCommand(id, request.ExpirationDate);
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteClassQuest(ISender sender, Guid id)
    {
        await sender.Send(new DeleteClassQuestCommand(id));
        return Results.NoContent();
    }

    public async Task<ClassQuestDto> GetClassQuestById(ISender sender, Guid id)
    {
        return await sender.Send(new GetClassQuestByIdQuery { Id = id });
    }

    public Task<List<ClassQuestDto>> GetClassQuests(ISender sender, Guid? classId = null, Guid? questId = null)
    {
        return sender.Send(new GetClassQuestsQuery { ClassId = classId, QuestId = questId });
    }
}
