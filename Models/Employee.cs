using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public class Employee
    {
        [Display(Name = "ID")]
        public int EmployeeId { get; set; } // código letras e números.


        [Required(ErrorMessage = "The Name is Required")]
        [Display(Name = "Nome")]
        public string? EmployeeName { get; set; }

        [Required(ErrorMessage = "The HiringDate is Required")]
        [Display(Name = "Data De Contratação")]
        public DateTime HiringDate { get; set; }

        [Display(Name = "Departamento")]
        public Department? Department { get; set; }

        [Display(Name = "Nível de Acesso")]
        public string? AcessLevel { get; set; } = "Funcionario"; // Adm

        // ter o acesso de adm ou n.

    }
}
