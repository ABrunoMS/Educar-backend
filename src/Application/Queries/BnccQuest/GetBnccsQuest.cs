using AutoMapper;
using AutoMapper.QueryableExtensions;
using Educar.Backend.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Queries.BnccQuest;

public class GetBnccsQuestQuery : IRequest<List<BnccQuestDto>>
{
    public Guid QuestId { get; init; }
}

public class GetBnccsQuestQueryHandler : IRequestHandler<GetBnccsQuestQuery, List<BnccQuestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBnccsQuestQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<BnccQuestDto>> Handle(GetBnccsQuestQuery request, CancellationToken cancellationToken)
    {
        return await _context.BnccQuests
            .AsNoTracking()
            .Include(bq => bq.Bncc)
            .Where(bq => bq.QuestId == request.QuestId)
            .OrderBy(bq => bq.Bncc.Description)
            .ProjectTo<BnccQuestDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}