namespace Educar.Backend.Application.Queries.Grade;

public class GradeDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Grade, GradeDto>();
        }
    }
}