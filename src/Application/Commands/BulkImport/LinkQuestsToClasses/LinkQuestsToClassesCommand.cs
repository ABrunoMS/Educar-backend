using Educar.Backend.Application.Commands.BulkImport;

namespace Educar.Backend.Application.Commands.BulkImport.LinkQuestsToClasses;

public record LinkQuestsToClassesCommand(Stream FileStream, string FileName) : IRequest<LinkQuestsToClassesResult>;
