namespace Educar.Backend.Domain.Entities;

// Tabela de ligação: Cliente <-> Conteúdo (Quais conteúdos o cliente comprou)
public class ClientContent
{
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
}