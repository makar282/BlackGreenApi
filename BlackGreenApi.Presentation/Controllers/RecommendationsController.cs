using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackGreenApi.Presentation.Controllers
{
	 [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class RecommendationsController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recommendation>>> GetRecommendations()
        {
            return await _context.Recommendations.ToListAsync();
        }
    }
}