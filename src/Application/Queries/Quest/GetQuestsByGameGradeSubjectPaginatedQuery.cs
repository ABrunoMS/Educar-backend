using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Quest;

public class GetQuestsByGameGradeSubjectPaginatedQuery : IRequest<PaginatedList<QuestCleanDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public Guid? GameId { get; init; }
    public Guid? GradeId { get; init; }
    public Guid? SubjectId { get; init; }
}

public class
    GetQuestsByGameGradeSubjectPaginatedQueryHandler : IRequestHandler<GetQuestsByGameGradeSubjectPaginatedQuery,
    PaginatedList<QuestCleanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetQuestsByGameGradeSubjectPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<QuestCleanDto>> Handle(GetQuestsByGameGradeSubjectPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Quests.AsQueryable();

        if (request.GameId is not null)
        {
            query = query.Where(q => q.GameId == request.GameId);
        }

        if (request.GradeId is not null)
        {
            query = query.Where(q => q.GradeId == request.GradeId);
        }

        if (request.SubjectId is not null)
        {
            query = query.Where(q => q.SubjectId == request.SubjectId);
        }

        return await query
            .OrderBy(q => q.Name)
            .ProjectTo<QuestCleanDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}