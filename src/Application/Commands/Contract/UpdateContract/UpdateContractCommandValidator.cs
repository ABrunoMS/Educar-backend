namespace Educar.Backend.Application.Commands.Contract.UpdateContract;

public class UpdateContractCommandValidator : AbstractValidator<UpdateContractCommand>
{
    public UpdateContractCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.ContractSigningDate)
            .NotEmpty().WithMessage("ContractSigningDate is required.");

        RuleFor(v => v.ImplementationDate)
            .NotEmpty().WithMessage("ImplementationDate is required.");

        RuleFor(v => v.ContractDurationInYears)
            .GreaterThan(0).WithMessage("ContractDurationInYears must be greater than 0.");

        RuleFor(v => v.TotalAccounts)
            .GreaterThan(0).WithMessage("TotalAccounts must be greater than 0.");

        RuleFor(v => v.Status)
            .IsInEnum().WithMessage("Status must be a valid ContractStatus.");
    }
}