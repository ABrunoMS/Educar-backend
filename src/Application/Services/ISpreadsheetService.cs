namespace Educar.Backend.Application.Services;

public interface ISpreadsheetService
{
    Task<List<T>> ReadSpreadsheetAsync<T>(Stream stream, string fileName) where T : class, new();
}
