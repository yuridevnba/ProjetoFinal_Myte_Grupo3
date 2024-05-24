using Microsoft.AspNetCore.Identity;

namespace ProjetoFinal_Myte_Grupo3.Areas.Admin.Models
{
    public class RoleEdit
    {
        public IdentityRole? Role { get; set; }
        public IEnumerable<IdentityUser> Members { get; set; }

        public IEnumerable<IdentityUser> ? NonMembers { get; set; }
    }
}
