using AutoMapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Appointment;
using DataAccessLayer.Entities.DTO.Herder;
using DataAccessLayer.Entities.DTO.Login;
using DataAccessLayer.Entities.DTO.Owner;
using DataAccessLayer.Entities.DTO.Pet;
using DataAccessLayer.Entities.DTO.Review;

namespace NanyPetAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // Herder
            CreateMap<Herder, HerderDto>().ReverseMap();
            CreateMap<Herder, HerderCreateDto>().ReverseMap();
            CreateMap<Herder, HerderUpdateDto>().ReverseMap();

            // Owner
            CreateMap<Owner, OwnerDto>().ReverseMap();
            CreateMap<Owner, OwnerCreateDto>().ReverseMap();
            CreateMap<Owner, OwnerUpdateDto>().ReverseMap();

            // User
            CreateMap<User, UserDto>().ReverseMap();

            // Pet
            CreateMap<Pet, PetDto>().ReverseMap();
            CreateMap<Pet, PetCreateDto>().ReverseMap();
            CreateMap<Pet, PetUpdateDto>().ReverseMap();

            // Appointment
            CreateMap<Appointment, AppointmentDto>().ReverseMap();
            CreateMap<Appointment, AppointmentCreateDto>().ReverseMap();

            // Review
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<Review, ReviewCreateDto>().ReverseMap();
        }
    }
}
