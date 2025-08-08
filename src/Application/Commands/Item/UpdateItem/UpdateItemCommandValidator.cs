using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Item.UpdateItem;

public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(v => v.Name)
            .MaximumLength(150).WithMessage("Name must not exceed 100 characters.")
            .NotEmpty().WithMessage("Name is required.");
        RuleFor(v => v.Lore).NotEmpty().WithMessage("Lore is required.");
        RuleFor(v => v.ItemType)
            .IsInEnum().WithMessage("ItemType must be a valid ItemType.")
            .NotEqual(ItemType.None).WithMessage("ItemType is required.");
        RuleFor(v => v.ItemRarity)
            .IsInEnum().WithMessage("ItemRarity must be a valid ItemRarity.")
            .NotEqual(ItemRarity.None).WithMessage("ItemRarity is required.");
        RuleFor(v => v.SellValue).NotEmpty().WithMessage("SellValue is required.")
            .PrecisionScale(10, 2, true)
            .WithMessage("SellValue must have a precision of 10 and a scale of 2.");
        RuleFor(v => v.Reference2D).NotEmpty().WithMessage("Reference2D is required.").MaximumLength(255);
        RuleFor(v => v.Reference3D).NotEmpty().WithMessage("Reference3D is required.").MaximumLength(255);
        RuleFor(v => v.DropRate).NotEmpty().WithMessage("DropRate is required.")
            .PrecisionScale(5, 2, true)
            .WithMessage("DropRate must have a precision of 5 and a scale of 2.");
    }
}