using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Grade;

public class GetGradeQuery : IRequest<GradeDto>
{
    public Guid Id { get; init; }
}

public class GetGradeQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetGradeQuery, GradeDto>
{
    public async Task<GradeDto> Handle(GetGradeQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Grades
            .ProjectTo<GradeDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Grade), request.Id.ToString());

        return mapper.Map<GradeDto>(entity);
    }
}