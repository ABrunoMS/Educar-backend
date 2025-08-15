using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Queries.Address;

public class GetAddressQuery : IRequest<AddressDto>
{
    public Guid Id { get; init; }
}

public class GetAddressQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetAddressQuery, AddressDto>
{
    public async Task<AddressDto> Handle(GetAddressQuery request, CancellationToken cancellationToken)
    {
        var entity = await context.Addresses
            .ProjectTo<AddressDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null) throw new NotFoundException(nameof(Domain.Entities.Address), request.Id.ToString());

        return mapper.Map<AddressDto>(entity);
    }
}
