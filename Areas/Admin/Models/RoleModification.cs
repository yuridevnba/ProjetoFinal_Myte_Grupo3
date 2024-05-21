using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal_Myte_Grupo3.Areas.Admin.Models
{
    public class RoleModification
    {
        [Required]
        public string? RoleName { get; set; }
        public string? RoleId { get; set; }
        public string[]? AddIds { get; set; }

        public string[]? DeleteIds { get; set; }


    }
}
