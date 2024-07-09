using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.Account.CreateAccount;

public record CreateAccountCommand(string Name, string Email, string RegistrationNumber, Guid ClientId, UserRole Role)
    : IRequest<CreatedResponseDto>
{
    public string Name { get; set; } = Name;
    public string Email { get; set; } = Email;
    public string RegistrationNumber { get; set; } = RegistrationNumber;
    public decimal AverageScore { get; set; }
    public decimal EventAverageScore { get; set; }
    public int Stars { get; set; }
    public Guid ClientId { get; set; } = ClientId;
    public UserRole Role { get; set; } = Role;
}

public class CreateAccountCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateAccountCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Clients.FindAsync([request.ClientId], cancellationToken: cancellationToken);
        if (client == null) throw new NotFoundException(nameof(Client), request.ClientId.ToString());

        var entity = new Domain.Entities.Account(request.Name, request.Email, request.RegistrationNumber, request.Role)
        {
            AverageScore = request.AverageScore,
            EventAverageScore = request.EventAverageScore,
            Stars = request.Stars,
            Client = client
        };

        entity.AddDomainEvent(new AccountCreatedEvent(entity));
        context.Accounts.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}