using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Grade.DeleteGrade;

public record DeleteGradeCommand(Guid Id) : IRequest<Unit>;

public class DeleteClientCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteGradeCommand, Unit>
{
    public async Task<Unit> Handle(DeleteGradeCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Grades
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        context.Grades.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}