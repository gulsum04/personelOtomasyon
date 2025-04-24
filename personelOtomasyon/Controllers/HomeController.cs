using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using personelOtomasyon.Models;
using System.Diagnostics;

namespace personelOtomasyon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "User"))
                        return RedirectToAction("Index", "Aday");

                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToAction("Dashboard", "Admin");

                    if (await _userManager.IsInRoleAsync(user, "Yonetici"))
                        return RedirectToAction("Dashboard", "Yonetici");

                    if (await _userManager.IsInRoleAsync(user, "Juri"))
                        return RedirectToAction("GelenBasvurular", "Juri");
                }
            }

            // Giriş yapılmamışsa ana sayfa gösterilir
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
