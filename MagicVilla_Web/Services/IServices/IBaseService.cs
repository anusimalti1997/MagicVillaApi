using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Response;

namespace MagicVilla_Web.Services.IServices
{
    public interface IBaseService
    {
         ApiResponse responseModal { get; set; }

        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
