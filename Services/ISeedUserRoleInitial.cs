namespace ProjetoFinal_Myte_Grupo3.Services
{
    public interface ISeedUserRoleInitial
    {
        Task SeedRolesAsync(); //Criar as roles
        Task SeedUserAsync();  //Criar os usuários, e atribuir o usuário a uma role que foi criada.
    }
}
