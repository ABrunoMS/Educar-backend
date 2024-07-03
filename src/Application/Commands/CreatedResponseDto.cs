namespace Educar.Backend.Application.Commands;

public class CreatedResponseDto(Guid id)
{
    public Guid Id { get; init; } = id;
}