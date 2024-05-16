using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public class Employee
    {
        [Display(Name = "ID")]
        public int EmployeeId { get; set; } // código letras e números.

        [Required(ErrorMessage = "The Name is Required")]
        [Display(Name = "Nome")]
        public string? EmployeeName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "The Email is Required")]
        public string? Email { get; set; }

        [Display(Name = "Senha")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The Password is Required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "The HiringDate is Required")]
        [Display(Name = "Data De Contratação")]
        public DateTime HiringDate { get; set; }

        [Display(Name = "Departamento")]
        public Department? Department { get; set; }

        [Display(Name = "Nível de Acesso")]
        public string? AcessLevel { get; set; } = "Funcionario"; // Adm

        [Display(Name = "Status")]
        public string? StatusEmployee { get; set; } = "Ativo"; // Inativo

        // ter o acesso de adm ou n.
    }
}