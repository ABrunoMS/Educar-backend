using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Client.CreateClient;

public record CreateClientCommand(string Name) : IRequest<IdResponseDto>
{
    public string? Description { get; init; }
}

public class CreateClientCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateClientCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Client(request.Name)
        {
            Description = request.Description,
        };

        context.Clients.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}