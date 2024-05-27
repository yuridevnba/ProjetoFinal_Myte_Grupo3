using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models.TelasLogin
{
    public class RegisterViewsModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string? Email { get; set; }



        [DataType(DataType.Password)]
        [Required]

<<<<<<< HEAD
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        [Required]
        public string? ConfirmPassword { get; set; }
        
=======
        public string? Password { get; set; } = GerarSenha(10);




        public static string GerarSenha(int comprimento)
        {
            const string caracteresPermitidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_+-=[]{}|;:,.<>?";
            const string digitos = "0123456789";
            Random random = new Random();
            char[] senha = new char[comprimento];

            // Insere pelo menos um dígito na senha
            senha[random.Next(comprimento)] = digitos[random.Next(digitos.Length)];

            // Insere outros caracteres na senha
            for (int i = 0; i < comprimento; i++)
            {
                if (senha[i] != '\0') continue; // Se já for um dígito, pula para o próximo caractere
                senha[i] = caracteresPermitidos[random.Next(caracteresPermitidos.Length)];
            }
            return new string(senha);
        }

        /// mudanças 
        /// 





>>>>>>> main
        [Display(Name = "ID")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Nome")]
        public string? EmployeeName { get; set; }
                
        public DateTime HiringDate { get; set; }

        [Display(Name = "Departamento")]
        public Department? Department { get; set; }

        [Display(Name = "Departamento")]
        public int DepartmentId { get; set; }

        [Display(Name = "Nível de Acesso")]
        public string? AcessLevel { get; set; } = "Employee"; //Standard

        [Display(Name = "Status")]
        public string? StatusEmployee { get; set; } = "Active"; // Inactive
    }
}
