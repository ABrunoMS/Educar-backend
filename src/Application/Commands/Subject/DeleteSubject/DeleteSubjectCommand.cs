using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Subject.DeleteSubject;

public record DeleteSubjectCommand(Guid Id) : IRequest<Unit>;

public class DeleteClassCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteSubjectCommand, Unit>
{
    public async Task<Unit> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Subjects
            .Include(a => a.GameSubjects)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        context.Subjects.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}