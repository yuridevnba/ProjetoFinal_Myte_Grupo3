using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal_Myte_Grupo3.Data;
using ProjetoFinal_Myte_Grupo3.Models;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Identity;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ProjetoFinal_Myte_Grupo3.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Employees
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string status)
        {
            var employees = _context.Employee.Include(e => e.Department)
                                             .AsQueryable();

            if (startDate.HasValue)
            {
                employees = employees.Where(e => e.HiringDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                employees = employees.Where(e => e.HiringDate.Date <= endDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status != "Todos")
                {
                    employees = employees.Where(e => e.StatusEmployee == status);
                }
            }
            else
            {
                employees = employees.Where(e => e.StatusEmployee == "Active");
            }

            var employeeList = await employees.ToListAsync();

            foreach (var employee in employeeList)
            {
                var user = await _userManager.FindByEmailAsync(employee.Email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    employee.AcessLevel = roles.Contains("Admin") ? "Admin" : "Standard";
                }
            }

            return View(employeeList);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.StatusEmployee = employee.StatusEmployee == "Active" ? "Inactive" : "Active";
            _context.Update(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //exportar relatório
        public async Task<IActionResult> Export(string[] fields, string sortOrder, string sortDirection)
        {
            var employeeData = await GetDataAsync(fields, sortOrder, sortDirection);

            using (XLWorkbook workBook = new XLWorkbook())
            {
                workBook.AddWorksheet(employeeData, "Dados Funcionário");

                using (MemoryStream ms = new MemoryStream())
                {
                    workBook.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "funcionarios.xlsx");
                }
            }
        }

        private async Task<DataTable> GetDataAsync(string[] fields, string sortOrder, string sortDirection)
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "Relatório Funcionários";

            // Adicionar colunas selecionadas
            foreach (var field in fields)
            {
                dataTable.Columns.Add(field, typeof(string));
            }

            var includeAccessLevel = fields.Contains("AcessLevel");

            var employees = _context.Employee.Include(e => e.Department).AsQueryable();

            // Aplicar ordenação
            if (!string.IsNullOrEmpty(sortOrder) && !string.IsNullOrEmpty(sortDirection))
            {
                employees = employees.OrderBy($"{sortOrder} {sortDirection}");
            }

            var employeeData = await employees.ToListAsync();

            if (employeeData.Count > 0)
            {
                foreach (var employee in employeeData)
                {
                    var user = await _userManager.FindByEmailAsync(employee.Email);
                    string accessLevel = "Standard";
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        accessLevel = roles.Contains("Admin") ? "Admin" : "Standard";
                    }

                    var row = dataTable.NewRow();
                    foreach (var field in fields)
                    {
                        if (field == "Department")
                        {
                            row[field] = employee.Department?.DepartmentName; // Ajuste conforme necessário
                        }
                        else
                        {
                            row[field] = employee.GetType().GetProperty(field)?.GetValue(employee, null)?.ToString();
                        }
                    }

                    if (includeAccessLevel)
                    {
                        row["AcessLevel"] = accessLevel;
                    }

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }



        //Verifica se o usuário está ativo para fazer login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Verifica se o usuário existe no banco de dados
            var user = await _context.Employee.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Verifica se o usuário está ativo
                if (user.StatusEmployee == "Active")
                {
                    // Usuário está ativo, redireciona para a página principal ou outra página desejada
                    // Aqui você pode redirecionar para a página que desejar após o login bem-sucedido
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Usuário está inativo, exibe uma mensagem de erro
                    ModelState.AddModelError("", "Usuário inativo. Entre em contato com o administrador.");
                }
            }
            else
            {
                // Usuário não encontrado, exibe uma mensagem de erro
                ModelState.AddModelError("", "Email ou senha incorretos.");
            }

            // Se chegou até aqui, significa que o login falhou, retorna para a página de login com as mensagens de erro
            return View("Login");
        }


        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(employee.Email);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                ViewData["AccessLevel"] = roles.Contains("Admin") ? "Admin" : "Standard";
            }
            else
            {
                ViewData["AccessLevel"] = "Standard";
            }

            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Department, "DepartmentId", "DepartmentName");
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeName,Email,Password,HiringDate,DepartmentId,AcessLevel,StatusEmployee")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Adiciona lógica para definir nível de acesso ao criar um funcionário
                var user = new IdentityUser { UserName = employee.Email, Email = employee.Email };
                var result = await _userManager.CreateAsync(user, employee.Password);
                if (result.Succeeded)
                {
                    if (employee.AcessLevel == "Admin")
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Standard");
                    }

                    employee.IdentityUserId = user.Id;
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "DepartmentId", "DepartmentName", employee.DepartmentId);
            return View(employee);
        }


        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "DepartmentId", "DepartmentName", employee.DepartmentId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,EmployeeName,Email,Password,HiringDate,DepartmentId,AccessLevel,StatusEmployee,IdentityUserId")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(employee.IdentityUserId);
                    if (user != null)
                    {
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        if (currentRoles.Contains("Admin") && employee.AcessLevel != "Admin")
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Admin");
                            await _userManager.AddToRoleAsync(user, "Standard");
                        }
                        else if (!currentRoles.Contains("Admin") && employee.AcessLevel == "Admin")
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Standard");
                            await _userManager.AddToRoleAsync(user, "Admin");
                        }


                        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user); // gera um token de redifinição de senha.
                        var result = await _userManager.ResetPasswordAsync(user, resetToken, employee.Password!);

                        _context.Update(employee);
                        await _context.SaveChangesAsync();
                    }


                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = employee.EmployeeId });

            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "DepartmentId", "DepartmentName", employee.DepartmentId);
            return View(employee);
        }



        [Authorize(Roles = "Admin")]
        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee != null)
            {
                _context.Employee.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.EmployeeId == id);
        }
    }
}