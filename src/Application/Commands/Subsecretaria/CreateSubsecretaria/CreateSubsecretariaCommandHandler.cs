using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using MediatR;

namespace Educar.Backend.Application.Commands.Subsecretaria.CreateSubsecretaria;

public class CreateSubsecretariaCommandHandler : IRequestHandler<CreateSubsecretariaCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateSubsecretariaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSubsecretariaCommand request, CancellationToken cancellationToken)
    {
    var entity = new Educar.Backend.Domain.Entities.Subsecretaria { Id = Guid.NewGuid(), Name = request.Name };
        _context.Subsecretarias.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
