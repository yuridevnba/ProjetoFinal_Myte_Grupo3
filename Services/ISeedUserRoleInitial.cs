namespace ProjetoFinal_Myte_Grupo3.Services
{
    public interface ISeedUserRoleInitial
    {
        Task SeedRolesAsync(); // criar as roles
        Task SeedUserAsync();  // criar os usuários, e atribuir o usuário a uma role que foi criada.
    }
}
