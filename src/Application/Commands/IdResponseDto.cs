namespace Educar.Backend.Application.Commands;

public class IdResponseDto(Guid id)
{
    public Guid Id { get; init; } = id;
}