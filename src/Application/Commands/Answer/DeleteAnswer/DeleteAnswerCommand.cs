using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Answer.DeleteAnswer;

public record DeleteAnswerCommand(Guid Id) : IRequest<Unit>;

public class DeleteAnswerCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteAnswerCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Answers
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        context.Answers.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}