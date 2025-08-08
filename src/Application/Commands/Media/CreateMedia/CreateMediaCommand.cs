using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.Media.CreateMedia;

public record CreateMediaCommand(
    string Name,
    string ObjectName,
    string Url,
    MediaPurpose Purpose,
    MediaType Type,
    bool Agreement,
    string? References = null,
    string? Author = null)
    : IRequest<IdResponseDto>;

public class CreateMediaCommandHandler(IApplicationDbContext context, IUser currentUser)
    : IRequestHandler<CreateMediaCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateMediaCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Media(request.Name, request.Purpose, request.Type, request.Agreement,
            request.Url, request.ObjectName)
        {
            References = request.References,
            Author = request.Author
        };

        if (currentUser.Id == null) throw new Exception("Couldn't get current user Id");

        entity.AddDomainEvent(new MediaCreatedEvent(entity, Guid.Parse(currentUser.Id)));
        context.Medias.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}