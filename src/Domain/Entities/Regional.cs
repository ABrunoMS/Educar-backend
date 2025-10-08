using System;

namespace Educar.Backend.Domain.Entities;

public class Regional
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Guid SubsecretariaId { get; set; }
    public Subsecretaria? Subsecretaria { get; set; }
}
