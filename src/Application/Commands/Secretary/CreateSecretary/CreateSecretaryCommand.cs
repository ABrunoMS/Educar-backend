using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Entities;
using MediatR;

namespace Educar.Backend.Application.Commands.Secretary.CreateSecretary;

public record CreateSecretaryCommand : IRequest<IdResponseDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Code { get; init; }
}

public class CreateSecretaryCommandHandler : IRequestHandler<CreateSecretaryCommand, IdResponseDto>
{
    private readonly IApplicationDbContext _context;

    public CreateSecretaryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IdResponseDto> Handle(CreateSecretaryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Secretary
        {
            Name = request.Name,
            Description = request.Description,
            Code = request.Code,
            IsActive = true
        };

        _context.Secretaries.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}
