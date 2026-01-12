using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Services;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.CreateQuestStepContent;

public record CreateQuestStepContentCommand(
    QuestStepContentType QuestStepContentType,
    QuestionType QuestionType,
    string Description,
    IAnswer Answers,
    decimal Weight,
    Guid QuestStepId)
    : IRequest<IdResponseDto>
{
    public QuestStepContentType QuestStepContentType { get; set; } = QuestStepContentType;
    public QuestionType QuestionType { get; set; } = QuestionType;
    public string Description { get; set; } = Description;
    public IAnswer Answers { get; set; } = Answers;
    public decimal Weight { get; set; } = Weight;
    public bool IsActive { get; set; } = true;
    public int Sequence { get; set; }
    public Guid QuestStepId { get; set; } = QuestStepId;
}

public class CreateQuestStepContentCommandHandler(
    IApplicationDbContext context,
    IQuestStepContentSequenceService sequenceService)
    : IRequestHandler<CreateQuestStepContentCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateQuestStepContentCommand request,
        CancellationToken cancellationToken)
    {
        var questStep = await context.QuestSteps
            .FirstOrDefaultAsync(qs => qs.Id == request.QuestStepId, cancellationToken);
        Guard.Against.NotFound(request.QuestStepId, questStep);

        // Se não informou sequência, pega a próxima disponível
        var sequence = request.Sequence > 0 
            ? request.Sequence 
            : await sequenceService.GetNextSequenceAsync(request.QuestStepId, cancellationToken);

        // Reorganizar sequências existentes se necessário
        await sequenceService.ReorderSequencesAsync(request.QuestStepId, sequence, null, cancellationToken);

        var entity = new Domain.Entities.QuestStepContent(request.QuestStepContentType,
            request.QuestionType, request.Description, request.Answers.ToJsonObject(), request.Weight)
        {
            QuestStep = questStep,
            IsActive = request.IsActive,
            Sequence = sequence
        };

        context.QuestStepContents.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}