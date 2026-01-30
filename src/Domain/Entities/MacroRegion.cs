using System.Collections.Generic;
using Educar.Backend.Domain.Common;

namespace Educar.Backend.Domain.Entities;

public class MacroRegion : BaseEntity
{
    public MacroRegion() { }

    public MacroRegion(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;

    public ICollection<Client> Clients { get; private set; } = new List<Client>();
}
