/*namespace Educar.Backend.Application.Commands.SpawnPoint.CreateSpawnPoint;

public class CreateSpawnPointCommandValidator : AbstractValidator<CreateSpawnPointCommand>
{
    public CreateSpawnPointCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

        RuleFor(v => v.Reference)
            .NotEmpty().WithMessage("Reference is required.");

        RuleFor(v => v.X)
            .NotEmpty().WithMessage("X coordinate is required.");

        RuleFor(v => v.Y)
            .NotEmpty().WithMessage("Y coordinate is required.");

        RuleFor(v => v.Z)
            .NotEmpty().WithMessage("Z coordinate is required.");

        RuleFor(v => v.MapId)
            .NotEmpty().WithMessage("MapId is required.");
    }
}*/