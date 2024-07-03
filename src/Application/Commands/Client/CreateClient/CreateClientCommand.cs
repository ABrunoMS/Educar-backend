using Educar.Backend.Application.Interfaces;

namespace Educar.Backend.Application.Commands.AccountType.CreateAccountType;

public class CreateClientCommand(string name, string description) : IRequest<Guid>
{
    public string Name { get; init; } = name;
    public string Description { get; init; } = description;
}

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateClientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Guid> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Guid.NewGuid());
    }
}