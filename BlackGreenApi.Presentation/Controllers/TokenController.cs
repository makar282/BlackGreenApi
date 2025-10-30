//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;

//namespace BlackGreenApi.Presentation.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class TokenController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<IdentityUser> _userManager;

//        public TokenController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        [HttpPost("set-apitoken")]
//        public async Task<IActionResult> SetApiToken([FromBody] dynamic request)
//        {
//            var apiToken = request.apiToken?.ToString();
//            if (string.IsNullOrEmpty(apiToken))
//            {
//                return BadRequest("API-токен не предоставлен");
//            }

//            var user = await _userManager.GetUserAsync(User);
//            if (user == null)
//            {
//                return Unauthorized("Пользователь не авторизован");
//            }

//            // Удаляем старый токен, если есть (для простоты храним один токен)
//            var existingToken = await _context.UserTokens
//                .FirstOrDefaultAsync(t => t.UserId == user.Id);
//            if (existingToken != null)
//            {
//                _context.UserTokens.Remove(existingToken);
//            }

//            // Сохраняем новый токен
//            var userToken = new UserToken
//            {
//                UserId = user.Id,
//                ApiToken = apiToken
//            };
//            _context.UserTokens.Add(userToken);
//            await _context.SaveChangesAsync();

//            return Ok();
//        }

//        [HttpGet("get-apitoken")]
//        public async Task<IActionResult> GetApiToken()
//        {
//            var user = await _userManager.GetUserAsync(User);
//            if (user == null)
//            {
//                return Unauthorized("Пользователь не авторизован");
//            }

//            var userToken = await _context.UserTokens
//                .FirstOrDefaultAsync(t => t.UserId == user.Id);
//            return Ok(new { apiToken = userToken?.ApiToken ?? "" });
//        }
//    }
//}