﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ProjetoFinal_Myte_Grupo3.Areas.Admin.Models;
using ProjetoFinal_Myte_Grupo3.Controllers;
using ProjetoFinal_Myte_Grupo3.Data;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing.Text;

namespace ProjetoFinal_Myte_Grupo3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;

        public AdminRolesController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            _context = context;

        }

        public ViewResult Index() => View(roleManager.Roles);

        [Route("Create")]
        public IActionResult Create()
        {
           return View();
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "AdminRoles");
                }
                else
                {
                    Errors(result);
                }
            }
            return View(name);
        }


        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var role = await roleManager.FindByNameAsync("Admin");
            List<IdentityUser> members = new List<IdentityUser>();
            List<IdentityUser> nonMembers = new List<IdentityUser>();

            foreach (IdentityUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name!) ? members : nonMembers; // ISInRoleAsync retorna true ou false. // ele recebe members ou nonmembers.
                list.Add(user);
            }
            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        private async Task UpdateAcessLevelAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);
                string accessLevel = roles.Contains("Admin") ? "Admin" : "Standard";

                var employee = await _context.Employee.FirstOrDefaultAsync(e => e.IdentityUserId == userId);
                if (employee != null)
                {
                    employee.AcessLevel = accessLevel;
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            IdentityResult result;

            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    IdentityUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            Errors(result);
                        }
                        else
                        {
                            await UpdateAcessLevelAsync(userId); // Atualiza o nível de acesso
                        }
                    }
                }

                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    IdentityUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            Errors(result);
                        }
                        else
                        {
                            await UpdateAcessLevelAsync(userId); // Atualiza o nível de acesso
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Update));
            }
            else
            {
                return await Update(model.RoleId);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ModelState.AddModelError("", "Role não encontrada");
                return View("Index", roleManager.Roles);
            }
            else
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
                return RedirectToAction("Index");
            }
            
            
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var role = await roleManager.FindByIdAsync(id);
        //    if (role != null)
        //    {
        //        IdentityResult result = await roleManager.DeleteAsync(role);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            Errors(result);
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Role não encontrada");
        //    }
        //    return View("Index",roleManager.Roles);
        //}
    }
}
// credenciais//login