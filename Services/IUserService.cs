using System.Threading.Tasks;
using SmartGym.Data;

namespace SmartGym.Services
{
    public interface IUserService
    {
         Task<ServiceResponse<string>> create(ProfileDto profileDto, int userId);
    }
}