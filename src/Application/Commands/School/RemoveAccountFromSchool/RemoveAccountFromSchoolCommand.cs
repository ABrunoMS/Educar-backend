using Educar.Backend.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Commands.School.RemoveAccountFromSchool;

public record RemoveAccountFromSchoolCommand(Guid SchoolId, Guid AccountId) : IRequest;

public class RemoveAccountFromSchoolCommandHandler : IRequestHandler<RemoveAccountFromSchoolCommand>
{
    private readonly IApplicationDbContext _context;

    public RemoveAccountFromSchoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoveAccountFromSchoolCommand request, CancellationToken cancellationToken)
    {
        var accountSchool = await _context.AccountSchools
            .FirstOrDefaultAsync(accountSchool => accountSchool.AccountId == request.AccountId && accountSchool.SchoolId == request.SchoolId, cancellationToken);

        if (accountSchool != null)
        {
            _context.AccountSchools.Remove(accountSchool);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
