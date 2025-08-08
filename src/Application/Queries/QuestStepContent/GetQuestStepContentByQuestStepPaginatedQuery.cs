using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.QuestStepContent;

public record GetQuestStepContentByQuestStepPaginatedQuery(Guid questStepId)
    : IRequest<PaginatedList<QuestStepContentDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class
    GetQuestStepContentByQuestStepPaginatedQueryHandler : IRequestHandler<GetQuestStepContentByQuestStepPaginatedQuery,
    PaginatedList<QuestStepContentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetQuestStepContentByQuestStepPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<QuestStepContentDto>> Handle(GetQuestStepContentByQuestStepPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.QuestStepContents
            .Where(n => n.QuestStepId == request.questStepId)
            .ProjectTo<QuestStepContentDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}