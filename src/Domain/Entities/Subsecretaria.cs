using System;
using System.Collections.Generic;

namespace Educar.Backend.Domain.Entities;

public class Subsecretaria
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public ICollection<Regional> Regionais { get; set; } = new List<Regional>();
}
