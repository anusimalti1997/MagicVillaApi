using AutoMapper;
using MagicVillaApi.Models.Villa;
using MagicVillaApi.Models.Villa.Dto;
using MagicVillaApi.Models.VillaNumber;
using MagicVillaApi.Models.VillaNumber.Dto;

namespace MagicVillaApi
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<Villa,VillaDTO>().ReverseMap();
            CreateMap<Villa,VillaCreateDTO>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();

            CreateMap<VillaNumber, VillaNumberDto>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberCreateDto>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdateDto>().ReverseMap();
        }
    }
}
