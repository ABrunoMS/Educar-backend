using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Answer;

public class GetAnswerQuery : IRequest<AnswerDto>
{
    public Guid Id { get; init; }
}

public class GetAnswerQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetAnswerQuery, AnswerDto>
{
    public async Task<AnswerDto> Handle(GetAnswerQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Answers
            .ProjectTo<AnswerDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Domain.Entities.Answer), request.Id.ToString());

        var AnswerDto = mapper.Map<AnswerDto>(entity);

        return AnswerDto;
    }
}