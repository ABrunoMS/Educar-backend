namespace Educar.Backend.Application.Commands.Media.UploadFileCommand;

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(x => x.File).NotEmpty().WithMessage("File is required.");
        RuleFor(x => x.Extension).NotEmpty().WithMessage("Extension is required.");
    }
}