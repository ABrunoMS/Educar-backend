using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using MediatR;

namespace Educar.Backend.Application.Commands.Regional.CreateRegional;

public class CreateRegionalCommandHandler : IRequestHandler<CreateRegionalCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateRegionalCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateRegionalCommand request, CancellationToken cancellationToken)
    {
    var entity = new Educar.Backend.Domain.Entities.Regional { Id = Guid.NewGuid(), Name = request.Name, SubsecretariaId = request.SubsecretariaId };
        _context.Regionais.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
