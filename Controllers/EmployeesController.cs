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

namespace ProjetoFinal_Myte_Grupo3.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;

        public EmployeesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.userManager = userManager;

        }

        // GET: Employees
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string status)
        {
            var employees = _context.Employee.AsQueryable();

            if (startDate.HasValue)
            {
                employees = employees.Where(e => e.HiringDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                employees = employees.Where(e => e.HiringDate <= endDate.Value);
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

            return View(await employees.ToListAsync());
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
        public IActionResult Export(string[] fields, string sortOrder, string sortDirection)
        {
            var employeeData = GetData(fields, sortOrder, sortDirection);

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

        private DataTable GetData(string[] fields, string sortOrder, string sortDirection)
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "Relatório Funcionários";

            // Adicionar colunas selecionadas
            foreach (var field in fields)
            {
                dataTable.Columns.Add(field, typeof(string)); // Você pode ajustar o tipo conforme necessário
            }

            var employees = _context.Employee.Include(e => e.Department).AsQueryable();

            // Aplicar ordenação
            if (!string.IsNullOrEmpty(sortOrder) && !string.IsNullOrEmpty(sortDirection))
            {
                employees = employees.OrderBy($"{sortOrder} {sortDirection}");
            }

            var employeeData = employees.ToList();

            if (employeeData.Count > 0)
            {
                foreach (var employee in employeeData)
                {
                    var row = dataTable.NewRow();
                    foreach (var field in fields)
                    {
                        if (field == "Department")
                        {
                            // Adicione aqui a lógica para obter o nome do departamento
                            row[field] = employee.Department?.DepartmentName; // Altere conforme necessário
                        }
                        else
                        {
                            row[field] = employee.GetType().GetProperty(field)?.GetValue(employee, null)?.ToString();
                        }
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

            var employee = await _context.Employee
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Department, "DepartmentId", "DepartmentName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeName,Email,Password,HiringDate,DepartmentId,AcessLevel,StatusEmployee")] Employee employee)
        {
            //employee.Email = "aaaa";
            //employee.Password = "dfda";

            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,EmployeeName,Email,Password,HiringDate,DepartmentId,AcessLevel,StatusEmployee")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "DepartmentId", "DepartmentName", employee.DepartmentId);
            return View(employee);
        }

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