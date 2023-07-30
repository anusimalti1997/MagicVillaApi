using System.ComponentModel.DataAnnotations;

namespace MagicVillaApi.Models.VillaNumber.Dto
{
    public class VillaNumberCreateDto
    {
        [Required]
        public int VillaNo { get; set; }
        [Required]
        public int VillaId { get; set; }
        public string SpecialDetails { get; set; }
    }
}
