using AutoMapper;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Queries.Bncc;

public class BnccDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Bncc, BnccDto>();
        }
    }
}