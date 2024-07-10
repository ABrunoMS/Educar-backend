using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.Account.DeleteAccount;

public record DeleteAccountCommand(Guid Id) : IRequest<Unit>;

public class DeleteAccountCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Accounts
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.AddDomainEvent(new AccountDeletedEvent(entity));

        context.Accounts.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}