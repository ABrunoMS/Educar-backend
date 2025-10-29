using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Contract.CreateContract;

public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(v => v.ClientId)
            .NotEmpty().WithMessage("ClientId is required.")
            .When(v => v.ClientId.HasValue);
        
        RuleFor(x => x.ProductIds) 
            .NotEmpty()
            .WithMessage("Pelo menos um 'ProductId' deve ser fornecido na lista.");

        RuleFor(v => v.ContentIds)
            .NotEmpty().WithMessage("Pelo menos um conteúdo deve ser selecionado.");

        RuleFor(v => v.ContractDurationInYears)
            .GreaterThan(0).WithMessage("ContractDurationInYears must be greater than 0.");

        RuleFor(v => v.ContractSigningDate)
            .NotEmpty().WithMessage("ContractSigningDate is required.");

        RuleFor(v => v.ImplementationDate)
            .NotEmpty().WithMessage("ImplementationDate is required.");

        RuleFor(v => v.TotalAccounts)
            .GreaterThan(0).WithMessage("TotalAccounts must be greater than 0.");

        RuleFor(v => v.Status)
            .IsInEnum().WithMessage("Status must be a valid ContractStatus.")
            .NotEqual(ContractStatus.None).WithMessage("Status is required.");
    }
}