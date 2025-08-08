using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Class;

public record GetClassQuery : IRequest<ClassDto>
{
    public Guid Id { get; init; }
}

public class GetClassQueryHandler : IRequestHandler<GetClassQuery, ClassDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClassQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClassDto> Handle(GetClassQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Classes
            .ProjectTo<ClassDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Class), request.Id.ToString());

        return _mapper.Map<ClassDto>(entity);
    }
}