using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Contract.DeleteContract;

public record DeleteContractCommand(Guid Id) : IRequest<Unit>;

public class DeleteContractCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteContractCommand, Unit>
{
    public async Task<Unit> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Contracts.FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        context.Contracts.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}