using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Dialogue.CreateDialogue;

public record CreateDialogueCommand(string Text, int Order, Guid NpcId) : IRequest<CreatedResponseDto>;

public class CreateDialogueCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateDialogueCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateDialogueCommand request, CancellationToken cancellationToken)
    {
        var npc = await context.Npcs.FindAsync([request.NpcId], cancellationToken: cancellationToken);
        if (npc == null) throw new NotFoundException(nameof(Npc), request.NpcId.ToString());

        var entity = new Domain.Entities.Dialogue(request.Text, request.Order)
        {
            Npc = npc
        };

        context.Dialogues.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}