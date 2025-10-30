//using Microsoft.AspNetCore.Mvc;
//using System.Diagnostics;

//namespace BlackGreenApi.Presentation.Controllers
//{
//	 public class HomeController(ILogger<HomeController> logger) : Controller
//    {
//        private readonly ILogger<HomeController> _logger = logger;

//        public IActionResult Dashboard()
//        {
//            return View("dashboard");
//        }

//        public IActionResult Login()
//        {
//            return View("login");
//        }

//        public IActionResult EditProfile()
//        {
//            return View("editProfile");
//        }

//        public IActionResult Profile()
//        {
//            return View("profile");
//        }

//        public IActionResult Admin()
//        {
//            return View("admin");
//        }

//        public IActionResult Purchases()
//        {
//            return View("purchases");
//        }

//        public IActionResult Threats()
//        {
//            return View("threats");
//        }

//        public IActionResult About()
//        {
//            return View("about");
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}
