﻿using MagicVillaApi.Models.ApplicationUser;
using MagicVillaApi.Models.Users;

namespace MagicVillaApi.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> IsUniqueUser(string userName);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegistrationRequestDTO registerRequestDTO);
    }
}
