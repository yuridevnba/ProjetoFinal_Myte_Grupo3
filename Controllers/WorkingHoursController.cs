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

        private async Task<(List<DateTime>, List<WBS>, List<List<int>>, List<int>)> GetWorkingHoursAsync(DateTime startDate)
        {
            var endDate = startDate.AddDays(14);
            var dateRange = Enumerable.Range(0, 15).Select(offset => startDate.AddDays(offset)).ToList();
            // TODO: Pegar working hours APENAS do employee logado! adicionar um where employeeId
            var workingHours = await _context.WorkingHour
                .Where(wh => wh.WorkedDate >= startDate && wh.WorkedDate <= endDate)
                .ToListAsync();

            var wbsList = await _context.WBS.ToListAsync();
            var workingHoursByWbsAndDate = new List<List<int>>();
            var totalsPerDay = new List<int>(new int[15]);

            foreach (var wbs in wbsList)
            {
                var hoursList = new List<int>();
                foreach (var date in dateRange)
                {
                    var hours = workingHours
                        .Where(wh => wh.WBSId == wbs.WBSId && wh.WorkedDate.Date == date.Date)
                        .Sum(wh => wh.WorkedHours);
                    hoursList.Add(hours);
                }
                workingHoursByWbsAndDate.Add(hoursList);
            }

            for (int dateIndex = 0; dateIndex < 15; dateIndex++)
            {
                totalsPerDay[dateIndex] = workingHoursByWbsAndDate.Sum(hoursList => hoursList[dateIndex]);
            }

            return (dateRange, wbsList, workingHoursByWbsAndDate, totalsPerDay);
        }

        public async Task<IActionResult> Index(DateTime? startDate)
        {
            var applicationDbContext = _context.WorkingHour.Include(w => w.Employee).Include(w => w.WBS);
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Code");

            var effectiveStartDate = startDate ?? DateTime.Today;

            var (dateRange, wbsList, workingHoursByWbsAndDate, totalsPerDay) = await GetWorkingHoursAsync(effectiveStartDate);

            ViewBag.DateRange = dateRange;
            ViewBag.WBSList = wbsList;
            ViewBag.WorkingHoursByWbsAndDate = workingHoursByWbsAndDate;
            ViewBag.TotalsPerDay = totalsPerDay;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveWorkingHours(List<int> WBSId, List<List<int>> Hours, List<DateTime> Dates)
        {
            // TODO: Ao invés de usar o numero 3 fixo, precisamos pegar o id do employee logado.
            var employeeId = 3;

            if (ModelState.IsValid)
            {
                // Dictionary to store the total sum of hours for each day
                var dailyTotalHours = new Dictionary<DateTime, int>();

                for (int i = 0; i < WBSId.Count; i++)
                {
                    for (int j = 0; j < Dates.Count; j++)
                    {
                        var wbsId = WBSId[i];
                        var date = Dates[j];
                        var hours = Hours[i][j];

                        // Only process if hours is greater than 0
                        if (hours > 0)
                        {
                            // Calculate the total sum of hours for each day
                            if (!dailyTotalHours.ContainsKey(date))
                            {
                                dailyTotalHours[date] = 0;
                            }
                            dailyTotalHours[date] += hours;

                            // Check if a working hour record already exists for this date, WBS, and employee
                            var existingWorkingHour = await _context.WorkingHour
                                .FirstOrDefaultAsync(wh => wh.WBSId == wbsId && wh.WorkedDate == date && wh.EmployeeId == employeeId);

                            if (existingWorkingHour != null)
                            {
                                // Update the existing record with the new worked hours
                                existingWorkingHour.WorkedHours = hours;
                                _context.Update(existingWorkingHour);
                            }
                            else
                            {
                                // Create a new working hour record
                                var newWorkingHour = new WorkingHour
                                {
                                    WBSId = wbsId,
                                    WorkedDate = date,
                                    WorkedHours = hours,
                                    EmployeeId = employeeId
                                };
                                _context.Add(newWorkingHour);
                            }
                        }
                    }
                }

                // Validate the total sum of hours for each day
                foreach (var kvp in dailyTotalHours)
                {
                    if (kvp.Value < 8)
                    {
                        TempData["ErrorMessage"] = $"Total sum of hours for {kvp.Key.ToString("yyyy-MM-dd")} is less than 8.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                // If validation passes, save the data
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View();
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
        public async Task<IActionResult> Create(List<WorkingHour> workingHours)
        {
            if (ModelState.IsValid)
            {
                foreach (var workingHour in workingHours)
                {
                    // Save each WorkingHour object to the database
                    _context.Add(workingHour);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // If model state is not valid, return to the create view with errors
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Code");
            return View(workingHours);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkingHourId,WBSId,WorkedDate,WorkedHours,EmployeeId")] WorkingHour workingHour)
        {
            if (id != workingHour.WorkingHourId || workingHour.WorkedHours < 8)
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
            ModelState.AddModelError(string.Empty, "O somatório das horas trabalhadas não pode ser menor que 8 horas.");
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