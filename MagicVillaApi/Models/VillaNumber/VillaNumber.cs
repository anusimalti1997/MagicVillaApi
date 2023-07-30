using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MagicVillaApi.Models.Villa;

namespace MagicVillaApi.Models.VillaNumber
{
    public class VillaNumber
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillaNo { get; set; }

        [ForeignKey("Villa")]
        public int VillaId { get; set; }
        public MagicVillaApi.Models.Villa.Villa Villa { get; set; }
        public string SpecialDetails { get; set; }
        public int Created { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
