using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using ProjetoFinal_Myte_Grupo3.Data;
using ProjetoFinal_Myte_Grupo3.Models;
using ProjetoFinal_Myte_Grupo3.Models.TelasLogin;
using ProjetoFinal_Myte_Grupo3.Services;
using System.Threading.Tasks;

namespace ProjetoFinal_Myte_Grupo3.Controllers
{

    public class AccountController : Controller
    {
        // Injeção de dependência.// facilita registro de usuários, logout
        private readonly UserManager<IdentityUser> userManager; //Crud de usuário login

        private readonly SignInManager<IdentityUser> signInManager; //Credenciais//login

        private readonly ApplicationDbContext _context;

        private readonly RegistersService registersService;

        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RegistersService registersService)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
            this.registersService = registersService;


        }
        public IActionResult Error()
        {

            
            return View();
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
                        Password = model.Password,
                    };

                    _context.Employee.Add(employee);
                    await _context.SaveChangesAsync();

                    var infos = new InfosEmployee
                    {
                        Salary = model.Salary,
                        Position = model.Position,
                        Cpf = model.Cpf,
                        Phone = model.Phone,
                        Cep = model.Cep,
                        Adress = model.Adress,
                        Number = model.Number,
                        City = model.City,
                        State = model.State,
                        EmployeeId = employee.EmployeeId,
                    };

                    _context.InfosEmployee.Add(infos);
                    await _context.SaveChangesAsync();

                    SendEmail.Send(model.Email, model.Password, "welcome");

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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == model.Email);
                    if (employee != null && employee.StatusEmployee == "Inactive")
                    {
                        ModelState.AddModelError(string.Empty, "Conta Inativa. Entre em contato com o administrador");
                        return View(model);
                    }

                    var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.Rememberme, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "WorkingHours");
                    }

                    ModelState.AddModelError(string.Empty, "Login Inválido");
                    }
                    else
                    {
                    ModelState.AddModelError(string.Empty, "Usuário ou senha incorretos.");
                    }
                }
            return View(model);
        }

        [HttpGet]
        public IActionResult EsqueceuSenha()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EsqueceuSenha(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userr = await userManager.FindByEmailAsync(model.Email);
                var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == model.Email);

                if (userr != null && employee != null)
                {
                    


                    var novaSenha = registersService.GerarSenha(10);
                    var resetToken = await userManager.GeneratePasswordResetTokenAsync(userr); // gera um token de redifinição de senha.
                    var result = await userManager.ResetPasswordAsync(userr, resetToken, novaSenha); // reseta a senha dele, colocando a nova senha.

                    //var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.Rememberme, false);

                    if (result.Succeeded)
                    {
                        employee.Password = novaSenha;
                        await _context.SaveChangesAsync();
                        SendEmail.Send(model.Email, novaSenha, "reset");

                        return RedirectToAction("Login", "Account");
                    }

                    ModelState.AddModelError(string.Empty, "Login Inválido");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário ou senha incorretos.");
                }
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

        [HttpGet]
        public async Task<IActionResult> SearchEmails(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false, message = "Query cannot be empty" });
            }

            var emails = await _context.Employee
                .Where(e => e.Email.Contains(query))
                .Select(e => e.Email)
                .ToListAsync();

            return Json(new { success = true, data = emails });
        }
    }
}