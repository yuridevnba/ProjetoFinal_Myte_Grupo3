using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoFinal_Myte_Grupo3.Models.TelasLogin;

namespace ProjetoFinal_Myte_Grupo3.Controllers
{

    public class AccountController : Controller
    {
        // Injeção de dependência.// facilita registro de usuários, logout
        private readonly UserManager<IdentityUser> userManager; // crud de usuário login

        private readonly SignInManager<IdentityUser> signInManager; // credenciais//login

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;

        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost] //Anotação de ENVIAR
        public async Task<IActionResult> Register(RegisterViewsModel model)// email e senha e confirmação
        { //recebe os dados do form    

            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {

                    UserName = model.Email,
                    Email = model.Email
                };



                //Armazena os dados do usuário na tabela AspNetUsers
                var result = await userManager.CreateAsync(user, model.Password!);

                // se o usuário foi criado com sucesso, faz o login do usuário
                // usando o serviço SignInManager e redireciona para o método Action Index.
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
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
                    return RedirectToAction("Index", "Home");
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





























