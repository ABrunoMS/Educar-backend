using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Map.CreateMap;

public class CreateMapCommandValidator : AbstractValidator<CreateMapCommand>
{
    public CreateMapCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.")
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(v => v.Type)
            .NotEmpty().WithMessage("Type is required.")
            .NotEqual(MapType.None).WithMessage("Type must be a valid enum value.");
        
        RuleFor(v => v.GameId).NotEmpty().WithMessage("GameId is required.");

        RuleFor(v => v.Reference2D).NotEmpty().WithMessage("Reference2D is required.").MaximumLength(255);
        RuleFor(v => v.Reference3D).NotEmpty().WithMessage("Reference3D is required.").MaximumLength(255);
    }
}