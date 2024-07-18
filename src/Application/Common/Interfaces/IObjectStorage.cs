namespace Educar.Backend.Application.Common.Interfaces;

public interface IObjectStorage
{
    Task<string?> PutObjectAsync(string objectName, Stream stream, CancellationToken cancellationToken);
}