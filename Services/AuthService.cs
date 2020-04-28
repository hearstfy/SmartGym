using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartGym.Data;
using SmartGym.Models;

namespace SmartGym.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext dataContext;
        private readonly IConfiguration configuration;
        public AuthService(DataContext dataContext, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.dataContext = dataContext;

        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();

            User user = await dataContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid user name or password.";

                return serviceResponse;
            }
            else
            {
                serviceResponse.Data = CreateToken(user);
            }
            return serviceResponse;
        }

        public Task<IActionResult> Logout()
        {
            throw new System.NotImplementedException();
        }

        public async Task<ServiceResponse<int>> Register(string username, string email, string password)
        {
            ServiceResponse<int> serviceResponse = new ServiceResponse<int>();

            if (await EmailExists(email))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This email already exists.";
                return serviceResponse;
            }

            if (await UserNameExists(username))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This user name already exists";
                return serviceResponse;
            }

            User user = new User()
            {
                Username = username,
                Email = email,
                RoleId = 1
            };

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await dataContext.Users.AddAsync(user);
            await dataContext.SaveChangesAsync();

            serviceResponse.Success = true;
            serviceResponse.Message = "Successfully registered";
            return serviceResponse;
        }

        public async Task<bool> UserNameExists(string username)
        {
            if (await dataContext.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> EmailExists(string email)
        {
            if (await dataContext.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim (ClaimTypes.Name, user.Username),
                    new Claim (ClaimTypes.Role, user.Role.RoleName)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}