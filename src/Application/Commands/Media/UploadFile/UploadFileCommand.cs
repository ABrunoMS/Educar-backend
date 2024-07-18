using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Media.UploadFileCommand;

public record UploadFileCommand(Stream File, string Extension) : IRequest<UploadResponseDto>;

public class UploadFileCommandHandler(IObjectStorage objectStorageService)
    : IRequestHandler<UploadFileCommand, UploadResponseDto>
{
    public async Task<UploadResponseDto> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var filename = $"{id}.{request.Extension}";
        var url = await objectStorageService.PutObjectAsync(filename, request.File, cancellationToken);
        Guard.Against.Null(url, "Failed to upload file to object storage.");

        return new UploadResponseDto(url, filename);
    }
}