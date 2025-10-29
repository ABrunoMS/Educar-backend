using AutoMapper;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Queries.Product;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Product, ProductDto>();
        }
    }
}