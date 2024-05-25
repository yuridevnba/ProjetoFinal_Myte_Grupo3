using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoFinal_Myte_Grupo3.Data;
using ProjetoFinal_Myte_Grupo3.Models;
using ProjetoFinal_Myte_Grupo3.Models.TelasLogin;

namespace ProjetoFinal_Myte_Grupo3.Controllers
{

    public class AccountController : Controller
    {
        // Injeção de dependência.// facilita registro de usuários, logout
        private readonly UserManager<IdentityUser> userManager; // crud de usuário login

        private readonly SignInManager<IdentityUser> signInManager; // credenciais//login

        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context,UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;

        }
        public IActionResult Register()
        {

            ViewData["DepartmentId"] = new SelectList(_context.Department, "DepartmentId", "DepartmentName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewsModel model)
        {
            // Recebe os dados do form    
            if (ModelState.IsValid)
            {

                

                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                // Armazena os dados do usuário na tabela AspNetUsers
                var result = await userManager.CreateAsync(user, model.Password!);

                // Se o usuário foi criado com sucesso, faz o login do usuário
                // usando o serviço SignInManager e redireciona para o método Action Index.
                if (result.Succeeded)
                {

                    var employee = new Employee
                    {
                        //EmployeeId = user.Id,
                        Email = model.Email!,
                        IdentityUserId = user.Id,
                        EmployeeName = model.EmployeeName,
                        HiringDate = model.HiringDate,
                        DepartmentId = model.DepartmentId,
                        AcessLevel = model.AcessLevel,
                        StatusEmployee = model.StatusEmployee,
                        Password = model.Password
                        
                    };


                    _context.Employee.Add(employee);
                    await _context.SaveChangesAsync();




                    await signInManager.SignInAsync(user, isPersistent: false);
                    // Retorna o resultado do redirecionamento para a action "Create" no controller "Employees"
                    return RedirectToAction("Index", "Employees");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)// email e senha e confirmação
        { //recebe os dados do form    

            if (ModelState.IsValid)
            {

                var result = await signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.Rememberme, false);



                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "WorkingHours");
                }

                ModelState.AddModelError(string.Empty, "Login Inválido");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("/Account/AccessDenied")]
        public ActionResult AccessDenied()
        {
            return View();
        }


    }

}





























