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

        public  string GerarSenha(int comprimento)
        {
            const string caracteresPermitidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_+-=[]{}|;:,.<>?";
            const string digitos = "0123456789";
            Random random = new Random();
            char[] senha = new char[comprimento];

            // Insere pelo menos um dígito na senha
            senha[random.Next(comprimento)] = digitos[random.Next(digitos.Length)];

            // Insere outros caracteres na senha
            for (int i = 0; i < comprimento; i++)
            {
                if (senha[i] != '\0') continue; // Se já for um dígito, pula para o próximo caractere
                senha[i] = caracteresPermitidos[random.Next(caracteresPermitidos.Length)];
            }
            return new string(senha);
        }









    }
}
