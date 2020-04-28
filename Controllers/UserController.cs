using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGym.Data;
using SmartGym.Models;
using SmartGym.Services;
using System.Threading.Tasks;

namespace SmartGym.Controllers
{
    [ApiController]
    [Route("profile")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;

        }
        [HttpPost("create")]
        public async Task<IActionResult> create(ProfileDto request)
        {
            ServiceResponse<string> response = await userService.create(request, getUserId());

            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        private int getUserId()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}