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

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WBS>().HasData(
                new WBS
                {
                    WBSId = 1,
                    Code = "WBS0000001",
                    Description = "Férias",
                    Type = "Non-Chargeability"
                },
                new WBS
                {
                    WBSId = 2,
                    Code = "WBS0000002",
                    Description = "Day-Off",
                    Type = "Non-Chargeability"
                },
                new WBS
                {
                    WBSId = 3,
                    Code = "WBS0000003",
                    Description = "Sem Tarefa",
                    Type = "Non-Chargeability"
                },
                new WBS
                {
                    WBSId = 4,
                    Code = "WBS0000004",
                    Description = "Implementação e Desenvolvimento",
                    Type = "Chargeability"
                }
            );
        }
    }
}

