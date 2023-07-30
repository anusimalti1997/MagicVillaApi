using MagicVillaApi.Models;
using MagicVillaApi.Models.Villa.Dto;

namespace MagicVillaApi.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO>
            {
                new VillaDTO{Id=1,Name="Pool villa",Occupancy=5,Sqft=800},
                new VillaDTO{Id=2,Name="Beach villa",Occupancy=7,Sqft=1400}

            };
    }
}
