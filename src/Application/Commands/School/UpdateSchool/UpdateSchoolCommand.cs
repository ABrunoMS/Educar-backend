using Educar.Backend.Application.Common.Interfaces;
using FluentValidation.Results;

namespace Educar.Backend.Application.Commands.School.UpdateSchool;

public record UpdateSchoolCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? AddressId { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? RegionalId { get; set; }
    public DateTime? ContractStartDate { get; set; }
}

public class UpdateSchoolCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateSchoolCommand, Unit>
{
    public async Task<Unit> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Schools
            .Include(s => s.Client)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.ContractStartDate.HasValue) entity.ContractStartDate = request.ContractStartDate;
        
        if (request.AddressId.HasValue)
        {
            var address = await context.Addresses.FindAsync([request.AddressId.Value], cancellationToken: cancellationToken);
            if (address != null)
            {
                entity.AddressId = request.AddressId.Value;
            }
        }

        if (request.ClientId.HasValue)
        {
            var client = await context.Clients.FindAsync([request.ClientId.Value], cancellationToken: cancellationToken);
            if (client != null)
            {
                entity.ClientId = request.ClientId.Value;
            }
        }

        if (request.RegionalId.HasValue)
        {
            // Primeiro, validar se a Regional existe
            var regional = await context.Regionais
                .Include(r => r.Subsecretaria)
                .FirstOrDefaultAsync(r => r.Id == request.RegionalId.Value, cancellationToken);
            
            if (regional == null)
            {
                throw new Educar.Backend.Application.Common.Exceptions.NotFoundException(nameof(Domain.Entities.Regional), request.RegionalId.Value.ToString());
            }

            // Depois de confirmar que a Regional é válida, validar se pertence ao Client correto
            var clientIdToValidate = request.ClientId ?? entity.ClientId;
            if (regional.Subsecretaria?.ClientId != clientIdToValidate)
            {
                var failures = new List<ValidationFailure>
                {
                    new ValidationFailure("RegionalId", "A Regional informada não pertence ao Client especificado.")
                };
                throw new Educar.Backend.Application.Common.Exceptions.ValidationException(failures);
            }
            
            entity.RegionalId = request.RegionalId.Value;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
