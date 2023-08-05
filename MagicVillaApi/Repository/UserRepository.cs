using MagicVillaApi.Data;
using MagicVillaApi.Models.Users;
using MagicVillaApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVillaApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;
        public UserRepository(ApplicationDbContext db,IConfiguration configuration)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<bool> IsUniqueUser(string userName)
        {
            var user = await _db.LocalUsers.FirstOrDefaultAsync(x=>x.UserName == userName);
            if(user == null)
            {
                return true;
            }
            return false;
            
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
           var user = await _db.LocalUsers.FirstOrDefaultAsync(u=>u.UserName.ToLower() == loginRequestDTO.UserName.ToLower() 
           && u.Password==loginRequestDTO.Password);

            if(user == null)
            {
                return null;
            }

            // if found then generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User=user
              
            };

        }

        public async Task<LocalUser> Register(RegistrationRequestDTO registerRequestDTO)
        {
            LocalUser user = new()
            {
                UserName=registerRequestDTO.UserName,
                Name=registerRequestDTO.Name,
                Password=registerRequestDTO.Password,
                Role=registerRequestDTO.Role,
            };

            await _db.LocalUsers.AddAsync(user);
            await _db.SaveChangesAsync();
            user.Password = "";
            return user;
        }
    }
}
