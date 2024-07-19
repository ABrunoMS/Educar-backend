using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Mappings;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Media;

public class GetMediaByPurposeAndTypePaginatedQuery : IRequest<PaginatedList<MediaDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public MediaPurpose? Purpose { get; init; }
    public MediaType? Type { get; init; }
}

public class GetMediaByPurposeAndTypePaginatedQueryHandler : IRequestHandler<GetMediaByPurposeAndTypePaginatedQuery,
    PaginatedList<MediaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMediaByPurposeAndTypePaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<MediaDto>> Handle(GetMediaByPurposeAndTypePaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Medias.AsQueryable();

        if (request.Purpose is not null)
        {
            query = query.Where(x => x.Purpose == request.Purpose);
        }

        if (request.Type is not null)
        {
            query = query.Where(x => x.Type == request.Type);
        }

        return await query
            .OrderBy(x => x.Name)
            .ProjectTo<MediaDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}