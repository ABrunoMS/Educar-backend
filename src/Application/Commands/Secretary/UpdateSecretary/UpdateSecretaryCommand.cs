using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using MediatR;

namespace Educar.Backend.Application.Commands.Secretary.UpdateSecretary;

public record UpdateSecretaryCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Code { get; init; }
    public bool IsActive { get; init; }
}

public class UpdateSecretaryCommandHandler : IRequestHandler<UpdateSecretaryCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSecretaryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSecretaryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Secretaries
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Domain.Entities.Secretary), request.Id.ToString());

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Code = request.Code;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
