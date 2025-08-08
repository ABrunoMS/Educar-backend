namespace Educar.Backend.Application.Common.Interfaces;

public interface IObjectStorage
{
    Task<string?> PutObjectAsync(string objectName, Stream stream, CancellationToken cancellationToken);
    Task<bool> DeleteObjectAsync(string objectName, CancellationToken cancellationToken);
    Task<bool> CheckObjectExistsAsync(string objectName, CancellationToken cancellationToken);
}