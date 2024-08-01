using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Dialogue.CreateDialogue;
using Educar.Backend.Application.Commands.Dialogue.DeleteDialogue;
using Educar.Backend.Application.Commands.Dialogue.UpdateDialogue;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Dialogue;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Dialogues : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPost(CreateDialogue)
            .MapGet(GetDialogue, "{id}")
            .MapGet(GetAllDialoguesByNpc, "npc/{npcId}")
            .MapPut(UpdateDialogue, "{id}")
            .MapDelete(DeleteDialogue, "{id}");
    }

    public Task<CreatedResponseDto> CreateDialogue(ISender sender, CreateDialogueCommand command)
    {
        return sender.Send(command);
    }

    public async Task<DialogueDto> GetDialogue(ISender sender, Guid id)
    {
        return await sender.Send(new GetDialogueQuery { Id = id });
    }

    public Task<PaginatedList<DialogueDto>> GetAllDialoguesByNpc(ISender sender, Guid npcId,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetDialoguesByNpcPaginatedQuery(npcId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteDialogue(ISender sender, Guid id)
    {
        await sender.Send(new DeleteDialogueCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateDialogue(ISender sender, Guid id, UpdateDialogueCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}