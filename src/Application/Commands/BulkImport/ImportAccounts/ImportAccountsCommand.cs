using Educar.Backend.Application.Commands.BulkImport;

namespace Educar.Backend.Application.Commands.BulkImport.ImportAccounts;

public record ImportAccountsCommand(Stream FileStream, string FileName) : IRequest<ImportAccountsResult>;
