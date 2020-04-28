using System;
using System.Threading.Tasks;
using AutoMapper;
using SmartGym.Data;
using SmartGym.Models;

namespace SmartGym.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper mapper;
        private readonly DataContext dataContext;
        public UserService(IMapper mapper, DataContext dataContext)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;

        }
        public async Task<ServiceResponse<string>> create(ProfileDto profileDto, int userId)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            UserProfile profile = mapper.Map<UserProfile>(profileDto);
            profile.UserId = userId;

            await dataContext.Profiles.AddAsync(profile);

            try
            {
                await dataContext.SaveChangesAsync();
                response.Success = true;
                response.Message = "Successfully created profile";
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Error occured during creating profile";
            }

            return response;
        }
    }
}