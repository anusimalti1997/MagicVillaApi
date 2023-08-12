using Microsoft.AspNetCore.Identity;

namespace MagicVillaApi.Models.ApplicationUser
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
