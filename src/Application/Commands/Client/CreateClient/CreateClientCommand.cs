using Educar.Backend.Application.Interfaces;

namespace Educar.Backend.Application.Commands.AccountType.CreateAccountType;

public record CreateClientCommand(string Name) : IRequest<CreatedResponseDto>
{
    public string? Description { get; init; }
}

public class CreateClientCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateClientCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Client(request.Name)
        {
            Description = request.Description,
        };

        context.Clients.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}