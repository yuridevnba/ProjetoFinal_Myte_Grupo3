using ProjetoFinal_Myte_Grupo3.Data;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoFinal_Myte_Grupo3.Services
{
    public class RegistersService
    {
        private readonly ApplicationDbContext dbContext;

        public RegistersService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<string> GetRegister()
        {
            var users = dbContext.Users.Select(u => u.Email).ToList();

            return users;
        }
    }
}
