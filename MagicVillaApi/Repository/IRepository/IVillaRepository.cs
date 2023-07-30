using MagicVillaApi.Models.Villa;
using System.Linq.Expressions;

namespace MagicVillaApi.Repository.IRepository
{
    public interface IVillaRepository : IRepository<Villa>
    {
       
        Task<Villa> UpdateAsync(Villa entity);
        
    }
}
