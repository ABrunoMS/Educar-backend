using Educar.Backend.Application.Common.Interfaces;
using NotFoundException = Educar.Backend.Application.Common.Exceptions.NotFoundException;

namespace Educar.Backend.Application.Commands.ClassQuest.DeleteClassQuest;

public record DeleteClassQuestCommand(Guid Id) : IRequest;

public class DeleteClassQuestCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteClassQuestCommand>
{
    public async Task Handle(DeleteClassQuestCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.ClassQuests
            .FirstOrDefaultAsync(cq => cq.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Domain.Entities.ClassQuest), request.Id.ToString());

        context.ClassQuests.Remove(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}
