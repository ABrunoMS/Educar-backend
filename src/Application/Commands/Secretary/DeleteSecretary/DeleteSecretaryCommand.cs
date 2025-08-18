using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using MediatR;

namespace Educar.Backend.Application.Commands.Secretary.DeleteSecretary;

public record DeleteSecretaryCommand(Guid Id) : IRequest;

public class DeleteSecretaryCommandHandler : IRequestHandler<DeleteSecretaryCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteSecretaryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSecretaryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Secretaries
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Domain.Entities.Secretary), request.Id.ToString());

        _context.Secretaries.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
