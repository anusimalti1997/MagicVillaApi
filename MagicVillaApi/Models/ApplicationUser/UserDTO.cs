namespace MagicVillaApi.Models.ApplicationUser
{
    public class UserDTO
    {
        //public string ID { get; set; }
        //public string UserName { get; set; }
        //public string Name { get; set; }

        public UserDTO User { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
