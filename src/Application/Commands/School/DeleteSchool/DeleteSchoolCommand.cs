using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.School.DeleteSchool;

public record DeleteSchoolCommand(Guid Id) : IRequest<Unit>;

public class DeleteSchoolCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteSchoolCommand, Unit>
{
    public async Task<Unit> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Schools
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (entity.Address != null) context.Addresses.Remove(entity.Address);

        context.Schools.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}