using DocumentFormat.OpenXml.Drawing.Diagrams;
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
        private readonly UserManager<IdentityUser> userManager; //Crud de usuário login

        private readonly SignInManager<IdentityUser> signInManager; //Credenciais//login

        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
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

                    SendEmail.Send(model.Email, model.Password);
                    //uzs21363@ilebi.com

                    return RedirectToAction("Index", "Employees");
                }
                else
                {
                    // Adiciona os erros ao ModelState para exibição na view
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Se chegamos aqui, algo falhou, retorna a view com os erros
            // Certifique-se de preencher os dados necessários para o campo "Departamento" novamente
            ViewBag.Departments = new SelectList(_context.Department, "DepartmentId", "DepartmentName");
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