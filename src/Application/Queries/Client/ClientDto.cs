using AutoMapper;
using Educar.Backend.Application.Queries.Contract; // Mantenha se você usar
using System;
using System.Collections.Generic;
using Educar.Backend.Domain.Entities;
using ClientEntity = Educar.Backend.Domain.Entities.Client;

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
    
    // Se você tiver uma lista de contratos associada, pode manter.
    // public List<ContractDto>? Contracts { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            // O AutoMapper mapeará automaticamente os novos campos se os nomes
            // corresponderem entre a Entidade de Domínio e o ClientDto.
            CreateMap<ClientEntity, ClientDto>()
                .ForMember(dest => dest.RemainingAccounts,
                           opt => opt.MapFrom(src => src.TotalAccounts - src.Accounts.Count()));
        }
    }
}