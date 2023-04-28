
using Microsoft.AspNetCore.Mvc;

namespace TemaCrm.Controllers
{
    public class PanelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
