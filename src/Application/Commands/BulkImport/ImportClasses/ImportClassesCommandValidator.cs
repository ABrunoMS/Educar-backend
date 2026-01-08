namespace Educar.Backend.Application.Commands.BulkImport.ImportClasses;

public class ImportClassesCommandValidator : AbstractValidator<ImportClassesCommand>
{
    public ImportClassesCommandValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("O arquivo é obrigatório.")
            .Must(stream => stream.Length > 0).WithMessage("O arquivo não pode estar vazio.")
            .Must(stream => stream.Length <= 10 * 1024 * 1024) // 10 MB
            .WithMessage("O arquivo não pode exceder 10 MB.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("O nome do arquivo é obrigatório.")
            .Must(fileName => fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            .WithMessage("O arquivo deve ser do tipo .xlsx");
    }
}
