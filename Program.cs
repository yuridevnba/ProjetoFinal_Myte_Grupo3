using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal_Myte_Grupo3.Data;
using ProjetoFinal_Myte_Grupo3.Models;
using ProjetoFinal_Myte_Grupo3.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 10;

    options.Password.RequiredUniqueChars = 3;

    options.Password.RequireNonAlphanumeric = false;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddCookie(options =>
 {
     options.Cookie.Name = "AspNetCore.Cookies";

     options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

     options.SlidingExpiration = true;
 });









builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUserAdminGerenteRole",
      policy => policy.RequireRole( "Admin"));

});

builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

await CriarPerfisUsuariosAsync(app);

app.UseAuthorization();


app.MapControllerRoute(
    name: "MinhaArea",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");





app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


async Task CriarPerfisUsuariosAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>(); // Interface como função gerênciar escopos do serviço para a aplicação.
    using (var scope = scopedFactory.CreateScope())
    {
        // createScope cria uma nova instância do serviço.
        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
        await service.SeedRolesAsync();
        await service.SeedUserAsync();
    }
}