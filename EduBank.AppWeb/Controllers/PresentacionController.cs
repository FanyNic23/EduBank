using Microsoft.AspNetCore.Mvc;

namespace EduBank.AppWeb.Controllers
{
    public class PresentacionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
