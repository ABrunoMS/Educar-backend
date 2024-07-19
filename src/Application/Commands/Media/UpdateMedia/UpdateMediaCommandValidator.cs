using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Media.UpdateMedia;

public class UpdateMediaCommandValidator : AbstractValidator<UpdateMediaCommand>
{
    private readonly IObjectStorage _objectStorage;

    public UpdateMediaCommandValidator(IObjectStorage objectStorage)
    {
        _objectStorage = objectStorage;

        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.").MaximumLength(100)
            .WithMessage("Name must be at most 100 characters.");
        RuleFor(x => x.ObjectName)
            .NotEmpty().WithMessage("ObjectName is required.")
            .MustAsync(ExistsInBucketAsync).WithMessage("ObjectName does not exist in the bucket.");
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Url is required.")
            .Must(BeAValidUrl).WithMessage("Url is not a valid URL.");
        RuleFor(x => x.Purpose).NotEmpty().WithMessage("Purpose is required.");
        RuleFor(x => x.Type).NotEmpty().WithMessage("Type is required.");
        RuleFor(x => x.Agreement).Equal(true).WithMessage("Agreement must be accepted.");
        RuleFor(x => x.Author).MaximumLength(100).WithMessage("Author must have at most 100 characters.");
    }

    private bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    private async Task<bool> ExistsInBucketAsync(string? objectName, CancellationToken cancellationToken)
    {
        if (objectName == null)
        {
            return false;
        }

        return await _objectStorage.CheckObjectExistsAsync(objectName, cancellationToken);
    }
}