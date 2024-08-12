using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.UpdateQuestStepContent;

public class UpdateQuestStepContentCommand : IRequest<CreatedResponseDto>
{
    public Guid Id { get; set; }
    public QuestStepContentType? QuestStepContentType { get; set; }
    public QuestionType? QuestionType { get; set; }
    public string? Description { get; set; }
    public IAnswer? ExpectedAnswers { get; set; }
    public decimal? Weight { get; set; }
}

public class UpdateQuestStepContentCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateQuestStepContentCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(UpdateQuestStepContentCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await context.QuestStepContents
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        entity.QuestionType = request.QuestionType ?? entity.QuestionType;
        entity.QuestStepContentType = request.QuestStepContentType ?? entity.QuestStepContentType;
        entity.Description = request.Description ?? entity.Description;
        entity.ExpectedAnswers = request.ExpectedAnswers?.ToJsonObject() ?? entity.ExpectedAnswers;
        entity.Weight = request.Weight ?? entity.Weight;

        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}