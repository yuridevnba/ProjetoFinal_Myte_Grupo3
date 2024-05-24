using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProjetoFinal_Myte_Grupo3.Areas.Admin.Controllers
{


    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public AdminUsersController(UserManager<IdentityUser> userManager)
        {

           
            this.userManager = userManager;

        }

        public IActionResult Index()
        {
            var users = userManager.Users;
            return View(users);
        }

    }

        
    }

