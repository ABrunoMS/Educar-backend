using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Events;
using MediatR;

namespace Educar.Backend.Application.Commands.Address.CreateAddress;

public record CreateAddressCommand : IRequest<IdResponseDto>
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public decimal? Lat { get; init; }
    public decimal? Lng { get; init; }
}

public class CreateAddressCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateAddressCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Address(
            request.Street,
            request.City,
            request.State,
            request.PostalCode,
            request.Country
        )
        {
            Lat = request.Lat,
            Lng = request.Lng
        };

        entity.AddDomainEvent(new AddressCreatedEvent(entity));
        context.Addresses.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}