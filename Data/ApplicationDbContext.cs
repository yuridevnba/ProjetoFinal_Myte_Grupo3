using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal_Myte_Grupo3.Models;

namespace ProjetoFinal_Myte_Grupo3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ProjetoFinal_Myte_Grupo3.Models.Employee> Employee { get; set; } = default!;
        public DbSet<ProjetoFinal_Myte_Grupo3.Models.WBS> WBS { get; set; } = default!;
        public DbSet<ProjetoFinal_Myte_Grupo3.Models.Department> Department { get; set; } = default!;
        public DbSet<ProjetoFinal_Myte_Grupo3.Models.WorkingHour> WorkingHour { get; set; } = default!;
    }
}
