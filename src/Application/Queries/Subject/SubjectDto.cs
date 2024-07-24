namespace Educar.Backend.Application.Queries.Subject;

public class SubjectDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Subject, SubjectDto>();
        }
    }
}