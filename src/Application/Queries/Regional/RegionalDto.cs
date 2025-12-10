using AutoMapper;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Queries.Regional;

public class RegionalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid SubsecretariaId { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Regional, RegionalDto>();
        }
    }
}
