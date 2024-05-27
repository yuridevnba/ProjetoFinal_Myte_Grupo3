using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Areas.Admin.Models
{
    public class RoleModification
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        public string? RoleName { get; set; }
        public string? RoleId { get; set; }
        public string[]? AddIds { get; set; }

        public string[]? DeleteIds { get; set; }


    }
}
