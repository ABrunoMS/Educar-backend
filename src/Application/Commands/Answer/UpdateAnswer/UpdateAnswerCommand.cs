using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using ValidationException = Educar.Backend.Application.Common.Exceptions.ValidationException;

namespace Educar.Backend.Application.Commands.Answer.UpdateAnswer;

public record UpdateAnswerCommand : IRequest<CreatedResponseDto>
{
    public Guid Id { get; set; }
    public IAnswer? GivenAnswer { get; set; }
    public bool? IsCorrect { get; set; }
}

public class UpdateAnswerCommandHandler : IRequestHandler<UpdateAnswerCommand, CreatedResponseDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateAnswerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreatedResponseDto> Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Answers
            .Include(a => a.QuestStepContent)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (entity.QuestStepContent.QuestionType != request.GivenAnswer?.QuestionType)
        {
            var exception = new ValidationException();
            exception.Errors.Add("GivenAnswer", ["Given QuestionType is different than expected by QuestStepContent"]);
            throw exception;
        }

        entity.GivenAnswer = request.GivenAnswer?.ToJsonObject() ?? entity.GivenAnswer;
        entity.IsCorrect = request.IsCorrect ?? entity.IsCorrect;

        await _context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}