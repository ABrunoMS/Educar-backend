using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.MediaLog.CreateMediaLog;

public class CreateMediaLogCommandValidator : AbstractValidator<CreateMediaLogCommand>
{
    public CreateMediaLogCommandValidator()
    {
        RuleFor(x => x.Action).NotEqual(AuditableAction.None).WithMessage("Action is required.");
        RuleFor(x => x.CurrentState).NotNull().WithMessage("CurrentState is required.");
        RuleFor(x => x.AccountId).NotEqual(Guid.Empty).WithMessage("AccountId is required.");
        RuleFor(x => x.MediaId).NotEqual(Guid.Empty).WithMessage("MediaId is required.");
        RuleFor(x => x.PreviousState).Must((x, y) => x.Action == AuditableAction.Update ? y != null : y == null)
            .WithMessage("PreviousState is required when Action is Update.");
    }
}