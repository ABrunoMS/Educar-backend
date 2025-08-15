using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Commands.School.CreateSchool;

public record CreateSchoolCommand(string Name, Guid ClientId) : IRequest<IdResponseDto>
{
    public string Name { get; set; } = Name;
    public string? Description { get; set; }
    public Guid? AddressId { get; set; }
    public Guid ClientId { get; set; } = ClientId;
}

public class CreateSchoolCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateSchoolCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Clients.FindAsync([request.ClientId], cancellationToken: cancellationToken);
        if (client == null) throw new Educar.Backend.Application.Common.Exceptions.NotFoundException(nameof(Client), request.ClientId.ToString());

        Guid? addressId = request.AddressId;

        var entity = new Domain.Entities.School(request.Name)
        {
            Description = request.Description,
            AddressId = addressId,
            Client = client
        };

        context.Schools.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}