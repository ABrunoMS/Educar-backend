namespace Educar.Backend.Application.Queries.Dialogue;

public class DialogueDto
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public int? Order { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Dialogue, DialogueDto>();
        }
    }
}