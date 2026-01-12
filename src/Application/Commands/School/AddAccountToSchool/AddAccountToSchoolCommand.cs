using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Application.Commands.School.AddAccountToSchool;

public record AddAccountToSchoolCommand(Guid SchoolId, Guid AccountId) : IRequest;

public class AddAccountToSchoolCommandHandler : IRequestHandler<AddAccountToSchoolCommand>
{
    private readonly IApplicationDbContext _context;

    public AddAccountToSchoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AddAccountToSchoolCommand request, CancellationToken cancellationToken)
    {
        // Verificar se o relacionamento já existe (incluindo registros soft-deleted)
        var exists = await _context.AccountSchools
            .IgnoreQueryFilters()
            .AnyAsync(accountSchool => accountSchool.AccountId == request.AccountId && accountSchool.SchoolId == request.SchoolId, cancellationToken);

        if (exists)
        {
            // Verificar se o registro está soft-deleted
            var existingRecord = await _context.AccountSchools
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(accountSchool => accountSchool.AccountId == request.AccountId && accountSchool.SchoolId == request.SchoolId, cancellationToken);

            if (existingRecord?.IsDeleted == true)
            {
                // Reativar o registro soft-deleted
                existingRecord.IsDeleted = false;
                existingRecord.DeletedAt = null;
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            throw new InvalidOperationException("Este usuário já está vinculado a esta escola.");
        }

        var accountSchool = new AccountSchool
        {
            AccountId = request.AccountId,
            SchoolId = request.SchoolId
        };

        _context.AccountSchools.Add(accountSchool);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
