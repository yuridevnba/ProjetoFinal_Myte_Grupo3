using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public class Employee
    {
        [Display(Name = "ID")]
        public int EmployeeId { get; set; } 

        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Nome")]
        public string? EmployeeName { get; set; }

        //[EmailAddress]
        //[Required(ErrorMessage = "Campo obrigatório")]
        public string? Email { get; set; }

        //[Display(Name = "Senha")]
        //[DataType(DataType.Password)]
        //[Required(ErrorMessage = "Campo obrigatório")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Data De Contratação")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HiringDate { get; set; }

        [Display(Name = "Departamento")]
        public Department? Department { get; set; }

        [Display(Name = "Departamento")]
        public int DepartmentId { get; set; }

        [Display(Name = "Nível de Acesso")]
        public string? AcessLevel { get; set; } = "Standard";

        [Display(Name = "Status")]
        public string? StatusEmployee { get; set; } = "Active"; // Inactive

        public string? IdentityUserId { get; set; }
    }
}