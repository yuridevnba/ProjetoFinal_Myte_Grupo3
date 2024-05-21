using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models.TelasLogin
{
    public class RegisterViewsModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        public string? ConfirmPassword { get; set; }

        public ICollection<string>? Emails { get; set; }
    }
}
