using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Media.CreateMedia;

public class CreateMediaCommandValidator : AbstractValidator<CreateMediaCommand>
{
    public CreateMediaCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.").MaximumLength(100)
            .WithMessage("Name must be at most 100 characters.");
        RuleFor(x => x.ObjectName).NotEmpty().WithMessage("ObjectName is required.");
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Url is required.")
            .Must(BeAValidUrl).WithMessage("Url is not a valid URL.");
        RuleFor(x => x.Purpose).NotEqual(MediaPurpose.None).WithMessage("Purpose is required.");
        RuleFor(x => x.Type).NotEqual(MediaType.None).WithMessage("Type is required.");
        RuleFor(x => x.Agreement).Equal(true).WithMessage("Agreement must be accepted.");
        RuleFor(x => x.Author).MaximumLength(100).WithMessage("Author must be at most 100 characters.");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}