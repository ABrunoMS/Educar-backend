using Educar.Backend.Application.Commands.BulkImport;

namespace Educar.Backend.Application.Commands.BulkImport.ImportClasses;

public record ImportClassesCommand(Stream FileStream, string FileName) : IRequest<ImportClassesResult>;
