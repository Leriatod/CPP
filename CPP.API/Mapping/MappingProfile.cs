using AutoMapper;
using CPP.API.Controllers.Dtos;
using CPP.API.Core.Models;

namespace CPP.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CarDto, Car>();
        }
    }
}