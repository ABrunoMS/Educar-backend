using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Exceptions;
using Educar.Backend.Domain.Entities;
using FluentValidation.Results;

namespace Educar.Backend.Application.Commands.School.CreateSchool;

public record CreateSchoolCommand(string Name, Guid ClientId, Guid RegionalId) : IRequest<IdResponseDto>
{
    public string Name { get; set; } = Name;
    public string? Description { get; set; }
    public Guid? AddressId { get; set; }
    public Guid ClientId { get; set; } = ClientId;
    public Guid RegionalId { get; set; } = RegionalId;
    public DateTime? ContractStartDate { get; set; }
}

public class CreateSchoolCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateSchoolCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Clients.FindAsync([request.ClientId], cancellationToken: cancellationToken);
        if (client == null) throw new Educar.Backend.Application.Common.Exceptions.NotFoundException(nameof(Client), request.ClientId.ToString());

        // Primeiro, validar se a Regional existe
        var regional = await context.Regionais
            .Include(r => r.Subsecretaria)
            .FirstOrDefaultAsync(r => r.Id == request.RegionalId, cancellationToken);
        if (regional == null) throw new Educar.Backend.Application.Common.Exceptions.NotFoundException(nameof(Regional), request.RegionalId.ToString());

        // Depois de confirmar que a Regional é válida, validar se pertence ao Client correto
        if (regional.Subsecretaria?.ClientId != request.ClientId)
        {
            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("RegionalId", "A Regional informada não pertence ao Client especificado.")
            };
            throw new Educar.Backend.Application.Common.Exceptions.ValidationException(failures);
        }

        Guid? addressId = request.AddressId;

        var entity = new Domain.Entities.School(request.Name)
        {
            Description = request.Description,
            AddressId = addressId,
            Client = client,
            Regional = regional,
            ContractStartDate = request.ContractStartDate
        };

        context.Schools.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}
