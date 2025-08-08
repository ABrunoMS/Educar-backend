using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.Media.DeleteMedia;

public record DeleteMediaCommand(Guid Id) : IRequest<Unit>;

public class DeleteMediaCommandHandler(IApplicationDbContext context, IObjectStorage objectStorage, IUser currentUser)
    : IRequestHandler<DeleteMediaCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.Id == null) throw new Exception("Couldn't get current user Id");

        var entity = await context.Medias
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        var success = await objectStorage.DeleteObjectAsync(entity.ObjectName, cancellationToken);
        if (!success)
        {
            throw new Exception("Failed to delete media object from bucket");
        }

        entity.AddDomainEvent(new MediaDeletedEvent(entity, Guid.Parse(currentUser.Id)));
        context.Medias.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}