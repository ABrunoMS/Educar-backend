using AutoMapper;
using Educar.Backend.Domain.Enums;
using System;
using AccountEntity = Educar.Backend.Domain.Entities.Account;

namespace Educar.Backend.Application.Queries.Account;

public class CleanAccountDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public UserRole Role { get; set; }
    public string? ClientName { get; set; }
    public string? SchoolName { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
           
            CreateMap<AccountEntity, CleanAccountDto>()
                .ForMember(dest => dest.ClientName,
                           opt => opt.MapFrom(src => src.Client.Name))
                .ForMember(dest => dest.SchoolName,
                           opt => opt.MapFrom(src => string.Join(", ", src.AccountSchools.Select(asc => asc.School.Name))));
        }
    }
}