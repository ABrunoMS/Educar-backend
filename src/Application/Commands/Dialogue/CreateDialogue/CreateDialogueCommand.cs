using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Dialogue.CreateDialogue;

public record CreateDialogueCommand(string Text, int Order, Guid NpcId) : IRequest<IdResponseDto>;

public class CreateDialogueCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateDialogueCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateDialogueCommand request, CancellationToken cancellationToken)
    {
        var npc = await context.Npcs.FindAsync([request.NpcId], cancellationToken: cancellationToken);
        if (npc == null) throw new NotFoundException(nameof(Npc), request.NpcId.ToString());

        var entity = new Domain.Entities.Dialogue(request.Text, request.Order)
        {
            Npc = npc
        };

        context.Dialogues.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}