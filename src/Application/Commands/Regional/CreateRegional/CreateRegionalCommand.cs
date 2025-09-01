using MediatR;

namespace Educar.Backend.Application.Commands.Regional.CreateRegional;

public class CreateRegionalCommand : IRequest<Guid>
{
    public string Nome { get; set; } = string.Empty;
    public Guid SubsecretariaId { get; set; }
}
