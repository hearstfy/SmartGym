using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SmartGym.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(string username, string email, string password);
        Task<ServiceResponse<string>> Login(string username, string password);
        Task<IActionResult> Logout();
        Task<bool> UserNameExists(string username);
        Task<bool> EmailExists(string email);
    }
}