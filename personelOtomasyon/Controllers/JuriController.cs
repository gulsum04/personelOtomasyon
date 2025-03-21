using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Juri")]
public class JuriController : Controller
{
    public IActionResult GelenBasvurular()
    {
        return View(); // Views/Juri/GelenBasvurular.cshtml
    }
}
