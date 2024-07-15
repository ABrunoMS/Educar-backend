using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Account.UpdateAccount;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator(IApplicationDbContext context)
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(v => v.RegistrationNumber)
            .NotEmpty().WithMessage("Registration number is required.");

        RuleFor(v => v.AverageScore)
            .InclusiveBetween(0, 999.99m).WithMessage("Average score must be between 0 and 999.99.")
            .PrecisionScale(5, 2, true)
            .WithMessage("Event average score must have up to 5 digits in total, with 2 decimal places.");

        RuleFor(v => v.EventAverageScore)
            .InclusiveBetween(0, 999.99m).WithMessage("Event average score must be between 0 and 999.99.")
            .PrecisionScale(5, 2, true)
            .WithMessage("Event average score must have up to 5 digits in total, with 2 decimal places.");

        RuleFor(v => v.Stars)
            .InclusiveBetween(0, 5).WithMessage("Stars must be between 0 and 5.");
    }
}