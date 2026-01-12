using Educar.Backend.Application.Common.Interfaces;
using NotFoundException = Educar.Backend.Application.Common.Exceptions.NotFoundException;

namespace Educar.Backend.Application.Queries.ClassQuest;

public record GetClassQuestByIdQuery : IRequest<ClassQuestDto>
{
    public Guid Id { get; init; }
}

public class GetClassQuestByIdQueryHandler : IRequestHandler<GetClassQuestByIdQuery, ClassQuestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClassQuestByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClassQuestDto> Handle(GetClassQuestByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.ClassQuests
            .Include(cq => cq.Class)
            .Include(cq => cq.Quest)
            .FirstOrDefaultAsync(cq => cq.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Domain.Entities.ClassQuest), request.Id.ToString());

        return _mapper.Map<ClassQuestDto>(entity);
    }
}
