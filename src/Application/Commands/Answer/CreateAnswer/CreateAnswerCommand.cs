using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Answer.CreateAnswer;

public class CreateAnswerCommand(IAnswer givenAnswer, Guid accountId, Guid questStepContentId, bool isCorrect)
    : IRequest<IdResponseDto>
{
    public Guid AccountId { get; set; } = accountId;
    public Guid QuestStepContentId { get; set; } = questStepContentId;
    public IAnswer GivenAnswer { get; set; } = givenAnswer;
    public bool IsCorrect { get; set; } = isCorrect;
}

public class CreateAnswerCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateAnswerCommand, IdResponseDto>
{
    public async Task<IdResponseDto> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        var questStepContent = await context.QuestStepContents
            .FirstOrDefaultAsync(qsc => qsc.Id == request.QuestStepContentId, cancellationToken);
        Guard.Against.NotFound(request.QuestStepContentId, questStepContent);
        
        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);
        Guard.Against.NotFound(request.AccountId, account);

        var entity = new Domain.Entities.Answer(request.GivenAnswer.ToJsonObject(), request.IsCorrect)
        {
            Account = account,
            QuestStepContent = questStepContent
        };

        context.Answers.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new IdResponseDto(entity.Id);
    }
}