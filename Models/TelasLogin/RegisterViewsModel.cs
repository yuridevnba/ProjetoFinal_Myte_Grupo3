using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models.TelasLogin
{
    public class RegisterViewsModel
    {
        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        public string? ConfirmPassword { get; set; }







        /// mudanças 
        /// 





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
