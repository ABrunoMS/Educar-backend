using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Services;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.UpdateQuestStepContent;

public class UpdateQuestStepContentCommand : IRequest<IdResponseDto>
{
    public Guid Id { get; set; }
    public QuestStepContentType? QuestStepContentType { get; set; }
    public QuestionType? QuestionType { get; set; }
    public string? Description { get; set; }
    public IAnswer? ExpectedAnswers { get; set; }
    public decimal? Weight { get; set; }
    public bool? IsActive { get; set; }
    public int? Sequence { get; set; }
}

public class UpdateQuestStepContentCommandHandler(
    IApplicationDbContext context,
    IQuestStepContentSequenceService sequenceService)
    : IRequestHandler<UpdateQuestStepContentCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(UpdateQuestStepContentCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await context.QuestStepContents
            .Where(a => a.Id == request.Id)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        // Se a sequência foi alterada, reorganizar
        if (request.Sequence.HasValue && request.Sequence.Value != entity.Sequence)
        {
            await sequenceService.ReorderSequencesAsync(
                entity.QuestStepId, 
                request.Sequence.Value, 
                entity.Id, 
                cancellationToken);
            entity.Sequence = request.Sequence.Value;
        }

        entity.QuestionType = request.QuestionType ?? entity.QuestionType;
        entity.QuestStepContentType = request.QuestStepContentType ?? entity.QuestStepContentType;
        entity.Description = request.Description ?? entity.Description;
        entity.ExpectedAnswers = request.ExpectedAnswers?.ToJsonObject() ?? entity.ExpectedAnswers;
        entity.Weight = request.Weight ?? entity.Weight;
        entity.IsActive = request.IsActive ?? entity.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}