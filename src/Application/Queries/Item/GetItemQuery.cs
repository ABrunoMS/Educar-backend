using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Item;

public record GetItemQuery : IRequest<ItemDto>
{
    public Guid Id { get; init; }
}

public class GetItemQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetItemQuery, ItemDto>
{
    public async Task<ItemDto> Handle(GetItemQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Items
            .ProjectTo<ItemDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Npc), request.Id.ToString());

        return mapper.Map<ItemDto>(entity);
    }
}