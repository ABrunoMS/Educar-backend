using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Events;

namespace Educar.Backend.Application.Commands.Address.CreateAddress;

public record CreateAddressCommand(string Street, string City, string State, string PostalCode, string Country)
    : IRequest<CreatedResponseDto>
{
    public string Street { get; set; } = Street;
    public string City { get; set; } = City;
    public string State { get; set; } = State;
    public string PostalCode { get; set; } = PostalCode;
    public string Country { get; set; } = Country;
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
}

public class CreateAddressCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateAddressCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Address(request.Street, request.City, request.State, request.PostalCode,
            request.Country)
        {
            Lat = request.Lat,
            Lng = request.Lng
        };

        entity.AddDomainEvent(new AddressCreatedEvent(entity));
        context.Addresses.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}