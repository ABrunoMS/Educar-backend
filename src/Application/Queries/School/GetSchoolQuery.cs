using Educar.Backend.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Queries.School;

public class GetSchoolQuery : IRequest<SchoolDto>
{
    public Guid Id { get; init; }
}

public class GetSchoolQueryHandler : IRequestHandler<GetSchoolQuery, SchoolDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSchoolQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SchoolDto> Handle(GetSchoolQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Schools
            .Include(s => s.AccountSchools)
                .ThenInclude(acs => acs.Account)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.School), request.Id.ToString());

        return _mapper.Map<SchoolDto>(entity);
    }
}