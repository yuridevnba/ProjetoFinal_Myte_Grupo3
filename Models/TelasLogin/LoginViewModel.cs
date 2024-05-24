using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models.TelasLogin
{
    public class LoginViewModel
    {


        [EmailAddress]
        [Required]
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool Rememberme { get; set; }

    }
}
