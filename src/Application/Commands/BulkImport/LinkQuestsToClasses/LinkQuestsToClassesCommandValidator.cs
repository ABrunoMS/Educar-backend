namespace Educar.Backend.Application.Commands.BulkImport.LinkQuestsToClasses;

public class LinkQuestsToClassesCommandValidator : AbstractValidator<LinkQuestsToClassesCommand>
{
    public LinkQuestsToClassesCommandValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("O arquivo é obrigatório.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("O nome do arquivo é obrigatório.")
            .Must(filename => filename.EndsWith(".xlsx") || filename.EndsWith(".xls"))
            .WithMessage("O arquivo deve ser do tipo Excel (.xlsx ou .xls).");

        RuleFor(x => x.FileStream)
            .Must(stream => stream != null && stream.Length > 0)
            .WithMessage("O arquivo não pode estar vazio.")
            .Must(stream => stream == null || stream.Length <= 10 * 1024 * 1024) // 10 MB
            .WithMessage("O arquivo não pode ter mais de 10 MB.");
    }
}
