using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Subject;

public class GetSubjectQuery: IRequest<SubjectDto>
{
    public Guid Id { get; init; }
}

public class GetSubjectQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetSubjectQuery, SubjectDto>
{
    public async Task<SubjectDto> Handle(GetSubjectQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Subjects
            .ProjectTo<SubjectDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Subject), request.Id.ToString());

        return mapper.Map<SubjectDto>(entity);
    }
}