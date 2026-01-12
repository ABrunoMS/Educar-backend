using Educar.Backend.Application.Commands.BulkImport;

namespace Educar.Backend.Application.Commands.BulkImport.LinkAccountsToClient;

public record LinkAccountsToClientCommand(Stream FileStream, string FileName) : IRequest<LinkAccountsToClientResult>;
