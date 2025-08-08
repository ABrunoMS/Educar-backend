using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Media;

public class GetMediaQuery : IRequest<MediaDto>
{
    public Guid Id { get; init; }
}

public class GetMediaQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetMediaQuery, MediaDto>
{
    public async Task<MediaDto> Handle(GetMediaQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Medias
            .ProjectTo<MediaDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Game), request.Id.ToString());

        return mapper.Map<MediaDto>(entity);
    }
}