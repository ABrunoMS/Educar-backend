using Educar.Backend.Application.Commands.Answer.CreateAnswer;
using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Answer.UpdateAnswer;

public class UpdateAnswerCommandValidator : AbstractValidator<UpdateAnswerCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAnswerCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id).NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.IsCorrect)
            .NotNull().WithMessage("IsCorrect is required."); // Changed to NotNull because it's a boolean

        RuleFor(v => v.GivenAnswer)
            .Must(ValidateGivenAnswer)
            .WithMessage("GivenAnswer is not valid.");
    }

    private bool ValidateGivenAnswer(UpdateAnswerCommand command, IAnswer? givenAnswer)
    {
        return AnswerTypeValidator.ValidateAnswer(givenAnswer?.QuestionType, givenAnswer);
    }
}