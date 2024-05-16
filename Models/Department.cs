using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public class Department
    {
        [Required]
        [Display(Name = "ID")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "The Name Departamento is Required")]
        [Display(Name = "Departamento")]
        public string? DepartmentName { get; set; }

        public ICollection<Employee>? Employee { get; set; }
    }
}