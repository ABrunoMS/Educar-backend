using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Address.DeleteAddress;

public record DeleteAddressCommand(Guid Id) : IRequest<Unit>;

public class DeleteAddressCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteAddressCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Addresses
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        context.Addresses.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}