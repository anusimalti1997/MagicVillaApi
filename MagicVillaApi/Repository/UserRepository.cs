using AutoMapper;
using MagicVillaApi.Data;
using MagicVillaApi.Models.ApplicationUser;
using MagicVillaApi.Models.Users;
using MagicVillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVillaApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext db,IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IMapper mapper
            , RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;

        }

        public async Task<bool> IsUniqueUser(string userName)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x=>x.UserName == userName);
            if(user == null)
            {
                return true;
            }
            return false;
            
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
           var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u=>u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if(user == null || isValid == false)
            {
                return null;
            }

            // if found then generate jwt token
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                Role=roles.FirstOrDefault()
              
            };

        }

        public async Task<UserDTO> Register(RegistrationRequestDTO registerRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName=registerRequestDTO.UserName,
                Email = registerRequestDTO.UserName,
                NormalizedEmail = registerRequestDTO.UserName.ToUpper(),
                Name =registerRequestDTO.Name,
              
            };

            try
            {
               var result=  await _userManager.CreateAsync(user,registerRequestDTO.Password);

                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("customer"));
                    }

                    await _userManager.AddToRoleAsync(user, "admin");
                    var userToReturn = await _db.ApplicationUsers.FirstOrDefaultAsync(x=>x.UserName==registerRequestDTO.UserName);

                    //return _mapper.Map<UserDTO>(userToReturn);

                    // if found then generate jwt token
                    var roles = await _userManager.GetRolesAsync(user);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(secretKey);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                    new Claim(ClaimTypes.Name,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,roles.FirstOrDefault())
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    return new UserDTO
                    {
                        Token = tokenHandler.WriteToken(token),
                        User = _mapper.Map<UserDTO>(user),
                        Role = roles.FirstOrDefault()
                    };
                }



            }catch(Exception ex)
            {
                return new UserDTO();
            }

            return new UserDTO();
        }
    }
}
