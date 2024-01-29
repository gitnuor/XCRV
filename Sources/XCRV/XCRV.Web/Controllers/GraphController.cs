using Microsoft.AspNetCore.Mvc;

namespace XCRV.Web.Controllers
{
    public class GraphController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
