using System;

namespace Educar.Backend.Application.Queries.Regional;

public class RegionalDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Guid SubsecretariaId { get; set; }
}
