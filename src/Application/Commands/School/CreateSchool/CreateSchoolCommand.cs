using Educar.Backend.Application.Commands.Address.CreateAddress;
using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.School.CreateSchool;

public record CreateSchoolCommand(string Name, Guid ClientId) : IRequest<CreatedResponseDto>
{
    public string Name { get; set; } = Name;
    public string? Description { get; set; }
    public Domain.Entities.Address? Address { get; set; }
    public Guid ClientId { get; set; } = ClientId;
}

public class CreateSchoolCommandHandler(IApplicationDbContext context, ISender sender)
    : IRequestHandler<CreateSchoolCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Clients.FindAsync([request.ClientId], cancellationToken: cancellationToken);
        if (client == null) throw new NotFoundException(nameof(Client), request.ClientId.ToString());

        Guid? addressId = null;
        if (request.Address != null)
        {
            var createAddressCommand = new CreateAddressCommand(request.Address.Street, request.Address.City,
                request.Address.State, request.Address.PostalCode, request.Address.Country)
            {
                Lat = request.Address.Lat,
                Lng = request.Address.Lng
            };
            var response = await sender.Send(createAddressCommand, cancellationToken);
            addressId = response.Id;
        }

        var entity = new Domain.Entities.School(request.Name)
        {
            Description = request.Description,
            AddressId = addressId,
            Client = client
        };

        context.Schools.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}