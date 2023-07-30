using MagicVillaApi.Models.Villa;
using MagicVillaApi.Models.VillaNumber;
using System.Linq.Expressions;

namespace MagicVillaApi.Repository.IRepository
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
       
        Task<VillaNumber> UpdateAsync(VillaNumber entity);
        
    }
}
