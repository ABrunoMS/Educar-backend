using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Npc.CreateNpc;

public class CreateNpcCommandValidator : AbstractValidator<CreateNpcCommand>
{
    public CreateNpcCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(150).WithMessage("Name must not exceed 100 characters.")
            .NotEmpty().WithMessage("Name is required.");
        RuleFor(v => v.Lore).NotEmpty().WithMessage("Lore is required.");
        RuleFor(v => v.NpcType)
            .IsInEnum().WithMessage("NpcType must be a valid NpcType.")
            .NotEqual(NpcType.None).WithMessage("NpcType is required.");
        RuleFor(v => v.GoldAmount).NotEmpty().WithMessage("GoldAmount is required.")
            .PrecisionScale(10, 2, true)
            .WithMessage("GoldAmount must have a precision of 10 and a scale of 2.");
        RuleFor(v => v.GoldDropRate).NotEmpty().WithMessage("GoldDropRate is required.")
            .PrecisionScale(5, 2, true)
            .WithMessage("GoldDropRate must have a precision of 5 and a scale of 2.");
    }
}