using AutoMapper;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Queries.Subsecretaria;

public class SubsecretariaDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ClientId { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Subsecretaria, SubsecretariaDto>();
        }
    }
}
