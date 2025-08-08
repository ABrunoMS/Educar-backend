using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Answer;

public record GetAnswerByAccountPaginatedQuery(Guid accountId)
    : IRequest<PaginatedList<AnswerDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetAnswerByQuestStepContentPaginatedQueryHandler :
    IRequestHandler<GetAnswerByAccountPaginatedQuery,
        PaginatedList<AnswerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAnswerByQuestStepContentPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<AnswerDto>> Handle(GetAnswerByAccountPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Answers
            .Where(n => n.AccountId == request.accountId)
            .ProjectTo<AnswerDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}