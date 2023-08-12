using MagicVillaApi.Models.ApplicationUser;

namespace MagicVillaApi.Models.Users
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
