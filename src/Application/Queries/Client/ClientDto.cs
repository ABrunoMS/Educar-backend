using AutoMapper;
using Educar.Backend.Application.Queries.Contract; // Mantenha se você usar
using System;
using System.Collections.Generic;
using Educar.Backend.Domain.Entities;
using ClientEntity = Educar.Backend.Domain.Entities.Client;
using Educar.Backend.Application.Queries.Product; 
using Educar.Backend.Application.Queries.Content;

namespace Educar.Backend.Application.Queries.Client;

public class ClientDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Partner { get; set; }
    public string? Contacts { get; set; }
    public string? Contract { get; set; }
    public string? Validity { get; set; }
    public string? SignatureDate { get; set; }
    public string? ImplantationDate { get; set; }
    public int TotalAccounts { get; set; }
    public int RemainingAccounts { get; set; }
    public string? Secretary { get; set; }
    public string? SubSecretary { get; set; }
    public string? Regional { get; set; }
    public IList<ProductDto> Products { get; set; } = new List<ProductDto>();
    public IList<ContentDto> Contents { get; set; } = new List<ContentDto>();
    
    // Se você tiver uma lista de contratos associada, pode manter.
    // public List<ContractDto>? Contracts { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            
            CreateMap<ClientEntity, ClientDto>()
                .ForMember(dest => dest.RemainingAccounts,
                           opt => opt.MapFrom(src => src.TotalAccounts - src.Accounts.Count()))

                .ForMember(dest => dest.Products,
                           opt => opt.MapFrom(src => src.ClientProducts.Select(cp => cp.Product)))
                
                
                .ForMember(dest => dest.Contents,
                           opt => opt.MapFrom(src => src.ClientContents.Select(cc => cc.Content)));
        
        }
    }
}