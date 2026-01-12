using OfficeOpenXml;

namespace Educar.Backend.Application.Services;

public class ExcelSpreadsheetService : ISpreadsheetService
{
    public async Task<List<T>> ReadSpreadsheetAsync<T>(Stream stream, string fileName) where T : class, new()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        if (stream == null || stream.Length == 0)
            throw new ArgumentException("O arquivo está vazio ou não foi fornecido.");

        if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("O arquivo deve ser do tipo .xlsx");

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        using var package = new ExcelPackage(memoryStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
            throw new InvalidOperationException("A planilha não contém abas.");

        var rows = new List<T>();
        var rowCount = worksheet.Dimension?.Rows ?? 0;

        if (rowCount < 2)
            throw new InvalidOperationException("A planilha não contém dados.");

        var properties = typeof(T).GetProperties();

        // Começa na linha 2 (assume header na linha 1)
        for (int row = 2; row <= rowCount; row++)
        {
            var obj = new T();

            for (int col = 0; col < properties.Length; col++)
            {
                var cellValue = worksheet.Cells[row, col + 1].Text;
                var property = properties[col];

                if (property.PropertyType == typeof(Guid))
                {
                    if (Guid.TryParse(cellValue, out var guidValue))
                        property.SetValue(obj, guidValue);
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(obj, cellValue);
                }
                // Adicione outros tipos conforme necessário
            }

            rows.Add(obj);
        }

        return rows;
    }
}
