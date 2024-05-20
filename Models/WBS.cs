using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public class WBS
    {
        [Required]
        [Display(Name = "ID")]
        public int WBSId { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string? Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Tipos")]
        public string? Type { get; set; }

    }
}