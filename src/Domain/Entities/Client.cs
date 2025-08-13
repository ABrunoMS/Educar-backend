using Educar.Backend.Domain.Common; // Importa o namespace da sua BaseEntity
using System.Collections.Generic;

namespace Educar.Backend.Domain.Entities;

public class Client : BaseEntity // <-- A herança correta para o seu projeto
{
    public Client(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Partner { get; set; }
    public string? Contacts { get; set; }
    public string? Contract { get; set; }
    public string? Validity { get; set; }
    public string? SignatureDate { get; set; }
    public string? ImplantationDate { get; set; }
    public int TotalAccounts { get; set; }
    public string? Secretary { get; set; }
    public string? SubSecretary { get; set; }
    public string? Regional { get; set; }
    public ICollection<Account> Accounts { get; private set; } = new List<Account>();
}