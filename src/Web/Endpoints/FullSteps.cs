using Educar.Backend.Application.Commands.QuestStep.CreateFullQuestStep;
// using Educar.Backend.Application.Commands.QuestStep.CreateQuestStep;
using Educar.Backend.Application.Commands.QuestStep.BulkCreateFullQuestStep;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Application.Commands;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class FullSteps : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPost(CreateFullQuestStep, "full")
            .MapPost(CreateFullQuestStepsInBulk, "full/bulk");
    }

    public Task<IdResponseDto> CreateFullQuestStep(ISender sender, CreateFullQuestStepCommand command)
    {
        return sender.Send(command);
    }
    public Task<IEnumerable<IdResponseDto>> CreateFullQuestStepsInBulk(ISender sender, BulkCreateFullQuestStepCommand command)
    {
        return sender.Send(command);
    }
}