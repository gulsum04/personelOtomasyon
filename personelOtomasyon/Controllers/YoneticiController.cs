using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Yonetici")]
public class YoneticiController : Controller
{
    public IActionResult Dashboard()
    {
        return View(); // Views/Yonetici/Dashboard.cshtml
    }
}
