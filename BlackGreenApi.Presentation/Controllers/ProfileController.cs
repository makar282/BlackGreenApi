//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace BlackGreenApi.Presentation.Controllers
//{
//	 [Authorize]
//    public class ProfileController(ApplicationDbContext context, IPasswordHasher passwordHasher, IJwtProvider jwtProvider) : Controller
//    {
//        private readonly ApplicationDbContext _context = context;
//        private readonly IPasswordHasher _passwordHasher = passwordHasher;
//        private readonly IJwtProvider _jwtProvider = jwtProvider;

//        public IActionResult Index()
//        {
//            // Логика для отображения текущей страницы профиля
//            var userName = User.Identity?.Name;
//            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

//            if (user == null)
//            {
//                return RedirectToAction("Login", "Account");
//            }

//            return View(new ProfileViewModel()); // Замените на вашу текущую логику
//        }

//        public IActionResult EditProfile()
//        {
//            var userName = User.Identity?.Name;
//            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

//            if (user == null)
//            {
//                return RedirectToAction("Login", "Account");
//            }

//            var model = new EditProfileViewModel
//            {
//                UserName = user.UserName,
//                Password = user.PasswordHash,
//                AboutMe = user.UserName
//            };

//            return View(model);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            // Находим текущего пользователя по User.Identity.Name
//            var userName = User.Identity?.Name;
//            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

//            if (user == null)
//            {
//                return RedirectToAction("Login", "Account");
//            }

//            // Обновляем имя пользователя
//            if (model.UserName != user.UserName)
//            {
//                user.UpdateUserName(model.UserName);
//            }

//            // Обновляем пароль (через твой сервис хеширования)
//            if (!string.IsNullOrEmpty(model.Password))
//            {
//                user.UpdatePasswordHash(_passwordHasher.Hash(model.Password));
//            }

//            // Обновляем поле AboutMe
//            if (model.AboutMe is not null)
//            {
//                var profile = await _context.AboutUsers.FirstOrDefaultAsync(p => p.UserId == user.Id);
//                if (profile is null)
//                {
//                    profile = new AboutUser { UserId = user.Id, AboutMe = model.AboutMe };
//                    _context.AboutUsers.Add(profile);
//                }
//                else
//                {
//                    profile.AboutMe = model.AboutMe;
//                }
//            }

//            // Сохраняем изменения
//            await _context.SaveChangesAsync();

//            // JWT-токен останется с прежними claims.
//            // Если имя пользователя изменилось — нужно выдать новый токен.
//            if (model.UserName != userName)
//            {
//                var token = _jwtProvider.GenerateToken(user);
//                Response.Cookies.Append("cok", token); // обновляем куку
//            }

//            return RedirectToAction("Index");
//        }
//    }
//}