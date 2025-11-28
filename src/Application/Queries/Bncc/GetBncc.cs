using AutoMapper;
using AutoMapper.QueryableExtensions;
using Educar.Backend.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Queries.Bncc;

public class GetAllBnccsQuery : IRequest<List<BnccDto>>
{
}

public class GetAllBnccsQueryHandler : IRequestHandler<GetAllBnccsQuery, List<BnccDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllBnccsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<BnccDto>> Handle(GetAllBnccsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Bnccs
            .AsNoTracking()
            .Where(b => b.IsActive)
            .OrderBy(b => b.Description)
            .ProjectTo<BnccDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}