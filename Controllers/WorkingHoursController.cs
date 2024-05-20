using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal_Myte_Grupo3.Data;
using ProjetoFinal_Myte_Grupo3.Models;

namespace ProjetoFinal_Myte_Grupo3.Controllers
{
    public class WorkingHoursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkingHoursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkingHours
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WorkingHour.Include(w => w.Employee).Include(w => w.WBS);
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Code");
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WorkingHours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingHour = await _context.WorkingHour
                .Include(w => w.Employee)
                .Include(w => w.WBS)
                .FirstOrDefaultAsync(m => m.WorkingHourId == id);
            if (workingHour == null)
            {
                return NotFound();
            }

            return View(workingHour);
        }

        // GET: WorkingHours/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "Email");
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Code");
            return View();
        }

        [HttpPost]
        [Route("api/workinghours/create")]
        public async Task<IActionResult> CreateWorkingHour([FromBody] WorkingHour workingHour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workingHour);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "WorkingHour created successfully" });
            }
            return BadRequest(new { success = false, message = "Invalid data" });
        }

        // POST: WorkingHours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkingHourId,WBSId,WorkedDate,WorkedHours,EmployeeId")] WorkingHour workingHour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workingHour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "Email", workingHour.EmployeeId);
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Code", workingHour.WBSId);
            return View(workingHour);
        }

        // GET: WorkingHours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingHour = await _context.WorkingHour.FindAsync(id);
            if (workingHour == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "Email", workingHour.EmployeeId);
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Code", workingHour.WBSId);
            return View(workingHour);
        }

        // POST: WorkingHours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkingHourId,WBSId,WorkedDate,WorkedHours,EmployeeId")] WorkingHour workingHour)
        {
            if (id != workingHour.WorkingHourId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workingHour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkingHourExists(workingHour.WorkingHourId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "Email", workingHour.EmployeeId);
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Code", workingHour.WBSId);
            return View(workingHour);
        }

        // GET: WorkingHours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingHour = await _context.WorkingHour
                .Include(w => w.Employee)
                .Include(w => w.WBS)
                .FirstOrDefaultAsync(m => m.WorkingHourId == id);
            if (workingHour == null)
            {
                return NotFound();
            }

            return View(workingHour);
        }

        // POST: WorkingHours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workingHour = await _context.WorkingHour.FindAsync(id);
            if (workingHour != null)
            {
                _context.WorkingHour.Remove(workingHour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkingHourExists(int id)
        {
            return _context.WorkingHour.Any(e => e.WorkingHourId == id);
        }
    }
}
