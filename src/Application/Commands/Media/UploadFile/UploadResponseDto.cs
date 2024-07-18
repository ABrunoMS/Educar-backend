namespace Educar.Backend.Application.Commands.Media.UploadFileCommand;

public class UploadResponseDto(string url, string objectName)
{
    public string Url { get; set; } = url;
    public string ObjectName { get; set; } = objectName;
}