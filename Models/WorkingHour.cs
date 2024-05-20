using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public class WorkingHour
    {
        [Display(Name = "ID")]
        public int WorkingHourId {  get; set; }

        [Display(Name = "WBS")]
        public WBS? WBS { get; set; }

        [Display(Name = "WBSId")]
        public int WBSId { get; set; }

        [Display(Name = "Dia Trabalhado")]
        public DateTime WorkedDate { get; set; }

        [Display(Name = "Horas Trabalhas")]
        public int WorkedHours { get; set; }

        [Display(Name = "Funcionario")]
        public Employee? Employee { get; set; }

        [Display(Name = "FuncionarioId")]
        public int EmployeeId{ get; set; }
    }
}
