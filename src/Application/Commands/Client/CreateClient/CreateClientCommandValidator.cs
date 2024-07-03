using Educar.Backend.Application.Interfaces;

namespace Educar.Backend.Application.Commands.AccountType.CreateAccountType;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    private IApplicationDbContext _context;

    public CreateClientCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.")
            .NotEmpty().WithMessage("Name is required.");
    }
}