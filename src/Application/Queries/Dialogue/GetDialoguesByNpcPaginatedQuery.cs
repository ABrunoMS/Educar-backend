using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;

namespace Educar.Backend.Application.Queries.Dialogue;

public record GetDialoguesByNpcPaginatedQuery(Guid NpcId) : IRequest<PaginatedList<DialogueDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetDialoguesByNpcPaginatedQueryHandler : IRequestHandler<GetDialoguesByNpcPaginatedQuery,
    PaginatedList<DialogueDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDialoguesByNpcPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<DialogueDto>> Handle(GetDialoguesByNpcPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Dialogues
            .Where(n => n.NpcId == request.NpcId)
            .OrderBy(n => n.Order)
            .ProjectTo<DialogueDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}