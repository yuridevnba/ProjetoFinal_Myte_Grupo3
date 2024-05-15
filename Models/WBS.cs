using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public class WBS
    {

        [Required]
        [Display(Name = "ID")]
        public string? WBSId { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string? Code { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Tipos")]
        public string? Type { get; set; }

        //public string? SubType { get; set; }
        // como as pessoas vão cadastrar wbs, escolhemos deixar uma tabela separada.

      
       

        //  *código
        //  *tipo
        //  *subtipos
        //  *descrição
        //  Id

        //-------------------------------------
        //Not chargeable
        //chargeable


    }
}
