//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace SaveNature.Controllers
//{
//    [AllowAnonymous]
//    public class UsersController : Controller
//    {
//        private readonly UserManager<IdentityUser> _userManager;

//        public UsersController(UserManager<IdentityUser> userManager)
//        {
//            _userManager = userManager;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var users = await _userManager.Users
//                .Select(u => new { u.Id, u.UserName, u.Email })
//                .ToListAsync();

//            return View(users);
//        }
//    }
//}