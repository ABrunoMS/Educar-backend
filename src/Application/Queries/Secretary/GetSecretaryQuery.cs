using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Queries.Secretary;

public class GetSecretaryQuery : IRequest<SecretaryDto>
{
    public Guid Id { get; init; }
}

public class GetSecretaryQueryHandler : IRequestHandler<GetSecretaryQuery, SecretaryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSecretaryQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SecretaryDto> Handle(GetSecretaryQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Secretaries
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Domain.Entities.Secretary), request.Id.ToString());

        return _mapper.Map<SecretaryDto>(entity);
    }
}
