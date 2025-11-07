using Educar.Backend.Application.Commands.QuestStep.CreateFullQuestStep;
// using Educar.Backend.Application.Commands.QuestStep.CreateQuestStep;
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
            .MapPost(CreateFullQuestStep, "full");
    }

    public Task<IdResponseDto> CreateFullQuestStep(ISender sender, CreateFullQuestStepCommand command)
    {
        return sender.Send(command);
    }
}