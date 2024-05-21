using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoFinal_Myte_Grupo3.Models;
using ProjetoFinal_Myte_Grupo3.Models.TelasLogin;
using ProjetoFinal_Myte_Grupo3.Services;

namespace ProjetoFinal_Myte_Grupo3.Controllers
{
    public class AccountController : Controller
    {
        //Injeção de dependência e facilita registro de usuários, logout
        private readonly UserManager<IdentityUser> userManager; //Crud de usuário login

        private readonly SignInManager<IdentityUser> signInManager; //Credenciais de login

        private readonly RegistersService registersService; //Método para lista de e-mails registrados

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RegistersService registersService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.registersService = registersService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost] //Anotação de ENVIAR
        public async Task<IActionResult> Register(RegisterViewsModel model)//E-mail e senha e confirmação
        { //Recebe os dados do form    
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                //Armazena os dados do usuário na tabela AspNetUsers
                var result = await userManager.CreateAsync(user, model.Password!);

                //Se o usuário foi criado com sucesso, faz o login do usuário
                //Usando o serviço SignInManager e redireciona para o método Action Index.
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    // Retorna o resultado do redirecionamento para a action "Create" no controller "Employees"
                    return RedirectToAction("Create", "Employees");
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)//Email e senha e confirmação
        { //Recebe os dados do form    

            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.Rememberme, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login Inválido");
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

        public IActionResult Users() 
        {
            var users = registersService.GetRegister(); ;

            return View(users);
        }
    }
}





























