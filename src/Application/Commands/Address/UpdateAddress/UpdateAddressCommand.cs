using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Address.UpdateAddress;

public record UpdateAddressCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
}

public class UpdateAddressCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateAddressCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Addresses
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.Street != null) entity.Street = request.Street;
        if (request.City != null) entity.City = request.City;
        if (request.State != null) entity.State = request.State;
        if (request.PostalCode != null) entity.PostalCode = request.PostalCode;
        if (request.Country != null) entity.Country = request.Country;
        if (request.Lat != null) entity.Lat = request.Lat;
        if (request.Lng != null) entity.Lng = request.Lng;

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}