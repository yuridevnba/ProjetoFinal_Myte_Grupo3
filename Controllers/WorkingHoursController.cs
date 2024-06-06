using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal_Myte_Grupo3.Data;
using ProjetoFinal_Myte_Grupo3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using iText.Kernel.Geom;
using iText.IO.Image;

namespace ProjetoFinal_Myte_Grupo3.Controllers
{
    public class WorkingHoursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkingHoursController(ApplicationDbContext context)
        {
            _context = context;
        }

        private List<(DateTime StartDate, DateTime EndDate)> Generate15DaySlices()
        {
            var startDate = new DateTime(2024, 1, 1);
            var slices = new List<(DateTime, DateTime)>();

            for (int i = 0; i < 24; i++) // Assume que precisa de 24 slices por um ano
            {
                var endDate = startDate.AddDays(14);
                slices.Add((startDate, endDate));
                startDate = endDate.AddDays(1); // Move para o próximo dia depois do final da fatia anterior (15 dias) 
            }
            return slices;
        }

        private (DateTime StartDate, DateTime EndDate) GetSliceFromDate(DateTime selectedDate)
        {
            var slices = Generate15DaySlices();
            foreach (var slice in slices)
            {
                if (selectedDate >= slice.StartDate && selectedDate <= slice.EndDate)
                {
                    return slice;
                }
            }
            // Se a seleção da data não é de uma fatia, retorne para a primeira fatia padrão (default)
            return slices.FirstOrDefault();
        }

        private async Task<(List<DateTime>, List<WBS>, List<List<int>>, List<int>)> GetWorkingHoursAsync(DateTime startDate, DateTime endDate)
        {
            var employeeId = GetCurrentEmployeeId();
            var dateRange = Enumerable.Range(0, (int)(endDate - startDate).TotalDays + 1)
                                      .Select(offset => startDate.AddDays(offset))
                                      .ToList();

            var workingHours = await _context.WorkingHour
                                             .Where(wh => wh.WorkedDate >= startDate && wh.WorkedDate <= endDate && wh.EmployeeId == employeeId)
                                             .ToListAsync();

            var workedWbsIds = new List<int>();

            workingHours.ForEach(workingHour =>
            {
                var wbsId = workingHour.WBSId;
                var resultadoDaBusca = workedWbsIds.FindIndex(workedWbsId => workedWbsId == wbsId);

                if (resultadoDaBusca == -1)
                {
                    workedWbsIds.Add(wbsId);
                }
            });

            var wbsList = await _context.WBS
                .Where(wbs => workedWbsIds.Contains(wbs.WBSId))
                .ToListAsync();
            var workingHoursByWbsAndDate = new List<List<int>>();
            var totalsPerDay = new List<int>(new int[dateRange.Count]);

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
            for (int dateIndex = 0; dateIndex < dateRange.Count; dateIndex++)
            {
                totalsPerDay[dateIndex] = workingHoursByWbsAndDate.Sum(hoursList => hoursList[dateIndex]);
            }
            return (dateRange, wbsList, workingHoursByWbsAndDate, totalsPerDay);
        }

        public async Task<IActionResult> Index(DateTime? selectedDate)
        {
            if (selectedDate == null)
            {
                selectedDate = DateTime.Today;
            }

            var employeeId = GetCurrentEmployeeId();

            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            ViewBag.EmployeeName = employee?.EmployeeName;
            ViewBag.EmployeeId = employee?.EmployeeId;

            DateTime effectiveStartDate;
            DateTime effectiveEndDate;

            if (selectedDate.HasValue)
            {
                var selectedDateTime = selectedDate.Value.Date;
                var selectedSlice = GetSliceFromDate(selectedDateTime);
                effectiveStartDate = selectedSlice.StartDate;
                effectiveEndDate = selectedSlice.EndDate;
            }
            else
            {
                var currentSlice = GetSliceFromDate(DateTime.Today);
                effectiveStartDate = currentSlice.StartDate;
                effectiveEndDate = currentSlice.EndDate;
            }

            var (dateRange, wbsList, workingHoursByWbsAndDate, totalsPerDay) = await GetWorkingHoursAsync(effectiveStartDate, effectiveEndDate);

            if (workingHoursByWbsAndDate.Count < 4)
            {
                for (var remainingLoops = 4 - workingHoursByWbsAndDate.Count; remainingLoops > 0; remainingLoops--)
                {
                    workingHoursByWbsAndDate.Add(Enumerable.Repeat(0, 15).ToList());
                }
            }
            ViewBag.DateRange = dateRange;
            ViewBag.WBSList = wbsList;
            ViewBag.AllWBSList = await _context.WBS
                .ToListAsync();
            ViewBag.WorkingHoursByWbsAndDate = workingHoursByWbsAndDate;
            ViewBag.TotalsPerDay = totalsPerDay;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveWorkingHours(List<int> WBSSelectedIdList, List<List<int>> Hours, List<DateTime> Dates)
        {
            var employeeId = GetCurrentEmployeeId();
            if (ModelState.IsValid)
            {
                var dailyTotalHours = CalculateDailyTotalHours(WBSSelectedIdList, Hours, Dates);
                if (!ValidateTotalHours(dailyTotalHours))
                {
                    return RedirectToAction(nameof(Index));
                }
                await SaveOrUpdateWorkingHours(WBSSelectedIdList, Hours, Dates, employeeId);
                TempData["SuccessMessage"] = "Suas horas foram salvas!";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private int GetCurrentEmployeeId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employee.FirstOrDefault(e => e.IdentityUserId == userId);
            return employee != null ? employee.EmployeeId : 0;
        }

        private Dictionary<DateTime, int> CalculateDailyTotalHours(List<int> WBSId, List<List<int>> Hours, List<DateTime> Dates)
        {
            var dailyTotalHours = new Dictionary<DateTime, int>();
            for (int i = 0; i < WBSId.Count; i++)
            {
                for (int j = 0; j < Dates.Count; j++)
                {
                    var date = Dates[j];
                    var hours = Hours[i][j];

                    if (hours > 0)
                    {
                        if (!dailyTotalHours.ContainsKey(date))
                        {
                            dailyTotalHours[date] = 0;
                        }
                        dailyTotalHours[date] += hours;
                    }
                }
            }
            return dailyTotalHours;
        }

        private bool ValidateTotalHours(Dictionary<DateTime, int> dailyTotalHours)
        {
            foreach (var kvp in dailyTotalHours)
            {
                if (kvp.Value < 8)
                {
                    TempData["ErrorMessage"] =
                          $" O somatório total de horas para a data {kvp.Key.ToString("yyyy-MM-dd")} deve ser maior que 8! ";
                    return false;
                }
                if (kvp.Value > 12)
                {
                    TempData["ErrorMessage"] =
                          $" O somatório total de horas para a data {kvp.Key.ToString("yyyy-MM-dd")} não deve ser maior que 12! ";
                    return false;
                }
            }
            return true;
        }

        private async Task SaveOrUpdateWorkingHours(List<int> WBSSelectedIdList, List<List<int>> Hours, List<DateTime> Dates, int employeeId)
        {
            for (int i = 0; i < WBSSelectedIdList.Count; i++)
            {
                for (int j = 0; j < Dates.Count; j++)
                {
                    var wbsId = WBSSelectedIdList[i];
                    var date = Dates[j];
                    var hours = Hours[i][j];
                    if (hours > 0 && wbsId != 0)
                    {
                        var existingWorkingHour = await _context.WorkingHour
                            .FirstOrDefaultAsync(wh => wh.WBSId == wbsId && wh.WorkedDate == date && wh.EmployeeId == employeeId);
                        if (existingWorkingHour != null)
                        {
                            existingWorkingHour.WorkedHours = hours;
                            _context.Update(existingWorkingHour);
                        }
                        else
                        {
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
            await _context.SaveChangesAsync();
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

        public async Task<IActionResult> GeneratePdfReport(DateTime? selectedDate)
        {
            var employeeId = GetCurrentEmployeeId();
            var employee = await _context.Employee.Include(e => e.Department) 
                                 .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            DateTime effectiveStartDate;
            DateTime effectiveEndDate;

            if (selectedDate.HasValue)
            {
                var selectedSlice = GetSliceFromDate(selectedDate.Value);
                effectiveStartDate = selectedSlice.StartDate;
                effectiveEndDate = selectedSlice.EndDate;
            }
            else
            {
                var currentSlice = GetSliceFromDate(DateTime.Today);
                effectiveStartDate = currentSlice.StartDate;
                effectiveEndDate = currentSlice.EndDate;
            }

            var workingHours = await _context.WorkingHour
                                             .Where(wh => wh.EmployeeId == employeeId && wh.WorkedDate >= effectiveStartDate && wh.WorkedDate <= effectiveEndDate)
                                             .Include(wh => wh.WBS)
                                             .ToListAsync();

            var dateRange = Enumerable.Range(0, (int)(effectiveEndDate - effectiveStartDate).TotalDays + 1)
                                      .Select(offset => effectiveStartDate.AddDays(offset))
                                      .ToList();

            var wbsList = workingHours.Select(wh => wh.WBS).Distinct().ToList();
            var workingHoursByWbsAndDate = new List<List<int>>();

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

            // Calculate the total hours for the entire period
            int totalHoursForPeriod = workingHours.Sum(wh => wh.WorkedHours);

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A4.Rotate());
                document.SetMargins(20, 20, 20, 20);

                string imagePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css/logo_mythree_test.png");

                ImageData imageData = ImageDataFactory.Create(imagePath);
                Image image = new Image(imageData).ScaleAbsolute(50, 50);

                document.Add(image);
                document.Add(new Paragraph(""));
                document.Add(new Paragraph(""));
                document.Add(new Paragraph($"Resumo do Funcionário: {employee.EmployeeName}").SetFontSize(12));
                document.Add(new Paragraph($"ID do Funcionário: {employee.EmployeeId}").SetFontSize(12));
                document.Add(new Paragraph($"Data de Contratação: {employee.HiringDate.ToString("dd/MM/yyyy")}").SetFontSize(12));
                document.Add(new Paragraph($"Departamento: {employee.Department?.DepartmentName}").SetFontSize(12));
                document.Add(new Paragraph(" ").SetFontSize(12));
                document.Add(new Paragraph("Horas Trabalhadas:").SetFontSize(12));

                
                var table = new Table(dateRange.Count + 2); 
                table.SetFontSize(8); 
                table.AddHeaderCell(new Cell().Add(new Paragraph("Código WBS").SetFontSize(8)));
                foreach (var date in dateRange)
                {
                    table.AddHeaderCell(new Cell().Add(new Paragraph(date.ToString("dd/MM/yyyy")).SetFontSize(8)));
                }
                table.AddHeaderCell(new Cell().Add(new Paragraph("Total").SetFontSize(8)));

                for (int i = 0; i < wbsList.Count; i++)
                {
                    var wbs = wbsList[i];
                    table.AddCell(new Cell().Add(new Paragraph(wbs.Code).SetFontSize(8)));
                    int totalHours = 0;
                    for (int j = 0; j < dateRange.Count; j++)
                    {
                        int hours = workingHoursByWbsAndDate[i][j];
                        table.AddCell(new Cell().Add(new Paragraph(hours.ToString()).SetFontSize(8)));
                        totalHours += hours;
                    }
                    table.AddCell(new Cell().Add(new Paragraph(totalHours.ToString()).SetFontSize(9)));
                }

                document.Add(table);
                document.Add(new Paragraph(" ").SetFontSize(12));
                document.Add(new Paragraph($"Total de horas da quinzena: {totalHoursForPeriod}").SetFontSize(12));

                document.Close();
                byte[] fileBytes = ms.ToArray();
                return File(fileBytes, "application/pdf", "Resumo_Funcionario.pdf");
            }
        }
    }
}
