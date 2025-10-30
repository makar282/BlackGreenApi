using BlackGreenApi.Application.Services.Interfaces;
using BlackGreenApi.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackGreenApi.Presentation.Controllers
{
	 [ApiController]
	 [Route("api/[controller]")]
	 [Authorize]
	 public class EcoRatingController : Controller
	 {
		  private readonly IEcoRatingManager _ecoRatingManager;
		  private readonly HttpClient _httpClient;


		  public EcoRatingController(IEcoRatingManager ecoRatingManager, HttpClient httpClient)
		  {
				_ecoRatingManager = ecoRatingManager;
				_httpClient = httpClient;
		  }

		  [HttpGet("get-ecorating")]
		  public async Task<IActionResult> GetEcoRating([FromBody] LoginUserRequest loginUserRequest)
		  {
				if (User?.Identity?.IsAuthenticated != true)
					 return BadRequest("User must be logged in");

				int ecoRating = await _ecoRatingManager.GetEcoRatingAsync(loginUserRequest.UserName);
				return Ok(new { EcoRating = ecoRating });
		  }
	 }
}