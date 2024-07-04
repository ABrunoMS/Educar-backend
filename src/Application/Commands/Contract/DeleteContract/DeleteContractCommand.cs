using Educar.Backend.Application.Interfaces;

namespace Educar.Backend.Application.Commands.Contract.DeleteContract;

public record DeleteContractCommand(Guid Id) : IRequest<Unit>;

public class DeleteContractCommandHandler : IRequestHandler<DeleteContractCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteContractCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Contracts.FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.Contracts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}