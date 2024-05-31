using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public class Department
    {
        [Required]
        [Display(Name = "ID")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Departamento")]
        public string? DepartmentName { get; set; }

        [Display(Name = "Quantidade de Funcionários")]
        public int EmployeeCount { get; set; }

        public ICollection<Employee>? Employee { get; set; }
    }
}