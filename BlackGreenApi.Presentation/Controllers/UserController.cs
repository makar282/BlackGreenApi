using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackGreenApi.Presentation.Controllers
{
	 [Route("api/[controller]")]
    [Authorize]
    public class UserController(IUserService userService, ApplicationDbContext dbContext) : Controller
    {
        private readonly IUserService _userService = userService;
        private readonly DbContext _dbContext = dbContext;

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            await _userService.Register(request.UserName, request.Password);
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var token = await _userService.Login(request.UserName, request.Password);
            return Ok(new { Token = token });
        }

        [HttpGet("get-ecorating")]
        public async Task<IActionResult> GetEcoRating(LoginUserRequest loginUserRequest)
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return BadRequest("User must be logged in");
            }

            int ecoRating = await _userService.GetEcoRatingAsync(loginUserRequest.UserName);
            return Ok(new { EcoRating = ecoRating });
        }
    }
}
