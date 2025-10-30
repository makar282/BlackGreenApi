using BlackGreenApi.Application.Services.Interfaces;
using BlackGreenApi.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackGreenApi.Presentation.Controllers
{
	 [ApiController]
	 [Route("api/[controller]")]
	 [Authorize]
	 public class UserController : Controller
	 {
		  private readonly IUserService _userService;

		  public UserController(IUserService userService, HttpClient httpClient)
		  {
				_userService = userService;
		  }

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
	 }
}