using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Queries.Secretary;

public class GetSecretariesPaginatedQuery : IRequest<PaginatedList<SecretaryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetSecretariesPaginatedQueryHandler : IRequestHandler<GetSecretariesPaginatedQuery, PaginatedList<SecretaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSecretariesPaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<SecretaryDto>> Handle(GetSecretariesPaginatedQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Secretaries
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Created)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<SecretaryDto>>(items);

        return new PaginatedList<SecretaryDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
