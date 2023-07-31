using AutoMapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Herder;
using DataAccessLayer.Entities.DTO.Login;
using DataAccessLayer.Entities.DTO.Owner;

namespace NanyPetAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // Herder Mappings
            CreateMap<Herder, HerderDto>().ReverseMap();
            CreateMap<Herder, HerderCreateDto>().ReverseMap();
            CreateMap<Herder, HerderUpdateDto>().ReverseMap();

            // Owner Mappings
            CreateMap<Owner, OwnerDto>().ReverseMap();
            CreateMap<Owner, OwnerCreateDto>().ReverseMap();
            CreateMap<Owner, OwnerUpdateDto>().ReverseMap();

            // User Mappings
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
