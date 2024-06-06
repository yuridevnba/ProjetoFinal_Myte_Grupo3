using Microsoft.AspNetCore.Mvc;

namespace SeuProjeto.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
