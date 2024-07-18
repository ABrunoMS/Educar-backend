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
    bool Agreement)
    : IRequest<CreatedResponseDto>
{
    public string? References;
    public string? Author;
}

public class CreateMediaCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateMediaCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateMediaCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Media(request.Name, request.Purpose, request.Type, request.Agreement,
            request.Url, request.ObjectName)
        {
            References = request.References,
            Author = request.Author
        };

        entity.AddDomainEvent(new MediaCreatedEvent(entity));
        context.Medias.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}