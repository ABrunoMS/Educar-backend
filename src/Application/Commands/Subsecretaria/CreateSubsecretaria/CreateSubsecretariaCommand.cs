using MediatR;

namespace Educar.Backend.Application.Commands.Subsecretaria.CreateSubsecretaria;

public record CreateSubsecretariaCommand(string Nome) : IRequest<Guid>;
