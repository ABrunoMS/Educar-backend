using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.Media.UpdateMedia;

public class UpdateMediaCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public MediaPurpose? Purpose { get; set; }
    public MediaType? Type { get; set; }
    public string? References { get; set; }
    public string? Author { get; set; }
    public bool? Agreement { get; set; }
    public string? Url { get; set; }
    public string? ObjectName { get; set; }
}

public class UpdateMediaCommandHandler(IApplicationDbContext context, IObjectStorage objectStorage)
    : IRequestHandler<UpdateMediaCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMediaCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Medias
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.ObjectName != null && !request.ObjectName.Equals(entity.ObjectName))
        {
            //versioning will do a 'soft delete' on bucket side of things
            await objectStorage.DeleteObjectAsync(entity.ObjectName, cancellationToken);
        }

        entity.Name = request.Name ?? entity.Name;
        entity.Purpose = request.Purpose ?? entity.Purpose;
        entity.Type = request.Type ?? entity.Type;
        entity.References = request.References ?? entity.References;
        entity.Author = request.Author ?? entity.Author;
        entity.Agreement = request.Agreement ?? entity.Agreement;
        entity.Url = request.Url ?? entity.Url;
        entity.ObjectName = request.ObjectName ?? entity.ObjectName;

        entity.AddDomainEvent(new MediaUpdatedEvent(entity));
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}