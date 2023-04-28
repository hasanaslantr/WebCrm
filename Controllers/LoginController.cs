using Microsoft.AspNetCore.Mvc;

namespace TemaCrm.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
