using Educar.Backend.Application.Commands.Address.UpdateAddress;
using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.School.UpdateSchool;

public record UpdateSchoolCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? RegionalId { get; set; }
    public Domain.Entities.Address? Address { get; set; }
}

public class UpdateSchoolCommandHandler(IApplicationDbContext context, ISender sender)
    : IRequestHandler<UpdateSchoolCommand, Unit>
{
    public async Task<Unit> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Schools
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.RegionalId.HasValue)
        {
            var regional = await context.Regionais.FindAsync([request.RegionalId.Value], cancellationToken: cancellationToken);
            if (regional != null)
            {
                entity.RegionalId = request.RegionalId.Value;
            }
        }

        if (request.Address != null)
        {
            var addressCommand = new UpdateAddressCommand
            {
                Id = request.Address.Id,
                Street = request.Address.Street,
                City = request.Address.City,
                State = request.Address.State,
                PostalCode = request.Address.PostalCode,
                Country = request.Address.Country,
                Lat = request.Address.Lat,
                Lng = request.Address.Lng,
            };
            await sender.Send(addressCommand, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
