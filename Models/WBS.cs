using ProjetoFinal_Myte_Grupo3.Data;
using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public class WBS
    {
        [Display(Name = "ID")]
        public int WBSId { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Código")]
        [StringLength(10)] // Define o comprimento máximo do código
        [RegularExpression(@"^[A-Za-z]{3}\d{7}$", ErrorMessage = "O código deve conter 3 letras seguidas de 7 números.")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Tipos")]
        public string? Type { get; set; }

        [Display(Name = "Contador")]
        public int SequentialCounter { get; set; } //Contador Sequencial

        public bool CodeExists(ApplicationDbContext context, string code)
        {
            return context.WBS.Any(wbs => wbs.Code == code);
        }

        public bool CodeExistsExcept(ApplicationDbContext context, string code, int id)
        {
            return context.WBS.Any(wbs => wbs.Code == code && wbs.WBSId != id);
        }
    }
}