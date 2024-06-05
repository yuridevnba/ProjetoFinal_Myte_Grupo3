using ProjetoFinal_Myte_Grupo3.Models.TelasLogin;
using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public class InfosEmployee
    {
        public int InfosEmployeeId { get; set; }

        [Display(Name = "Salário")]
        public string? Salary { get; set; }

        [Display(Name = "Cargo")]
        public string? Position { get; set; }

        [Display(Name = "CPF")]
        public string? Cpf { get; set; }

        [Display(Name = "Telefone")]
        public string? Phone { get; set; }

        [Display(Name = "CEP")]
        public string? Cep { get; set; }

        [Display(Name = "Endereço")]
        public string? Adress { get; set; }

        [Display(Name = "Número")]
        public int Number { get; set; }

        [Display(Name = "Cidade")]
        public string? City { get; set; }

        [Display(Name = "Estado")]
        public string? State { get; set; }

        public Employee? Employee { get; set; }

        public int EmployeeId { get; set; }
    }
}
