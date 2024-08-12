using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.CreateQuestStepContent;

public record CreateQuestStepContentCommand(
    QuestStepContentType QuestStepContentType,
    QuestionType QuestionType,
    string Description,
    IAnswer Answers,
    decimal Weight,
    Guid QuestStepId)
    : IRequest<CreatedResponseDto>
{
    public QuestStepContentType QuestStepContentType { get; set; } = QuestStepContentType;
    public QuestionType QuestionType { get; set; } = QuestionType;
    public string Description { get; set; } = Description;
    public IAnswer Answers { get; set; } = Answers;
    public decimal Weight { get; set; } = Weight;
    public Guid QuestStepId { get; set; } = QuestStepId;
}

public class CreateQuestStepContentCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateQuestStepContentCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateQuestStepContentCommand request,
        CancellationToken cancellationToken)
    {
        var questStep = await context.QuestSteps
            .FirstOrDefaultAsync(qs => qs.Id == request.QuestStepId, cancellationToken);
        Guard.Against.NotFound(request.QuestStepId, questStep);

        var entity = new Domain.Entities.QuestStepContent(request.QuestStepContentType,
            request.QuestionType, request.Description, request.Answers.ToJsonObject(), request.Weight)
        {
            QuestStep = questStep
        };

        context.QuestStepContents.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}