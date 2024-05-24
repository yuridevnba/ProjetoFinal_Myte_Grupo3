using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal_Myte_Grupo3.Data;
using ProjetoFinal_Myte_Grupo3.Models;

namespace ProjetoFinal_Myte_Grupo3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WBSController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WBSController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WBS
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.WBS.ToListAsync());
        //}

        public async Task<IActionResult> Index(string[] type)
        {
            IQueryable<WBS> types = _context.WBS;

            if (type != null && type.Length > 0)
            {
                types = types.Where(t => type.Contains(t.Type));
            }

            return View(await types.ToListAsync());
        }


        // GET: WBS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wBS = await _context.WBS
                .FirstOrDefaultAsync(m => m.WBSId == id);
            if (wBS == null)
            {
                return NotFound();
            }

            return View(wBS);
        }

        // GET: WBS/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WBS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WBSId,Code,Description,Type")] WBS wBS)
        {
            if (wBS.Code != null && wBS.CodeExists(_context, wBS.Code))
            {
                ModelState.AddModelError("Code", "Este código já existe. Por favor, insira um código diferente.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(wBS);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wBS);
        }

        // GET: WBS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wBS = await _context.WBS.FindAsync(id);
            if (wBS == null)
            {
                return NotFound();
            }
            return View(wBS);
        }

        // POST: WBS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WBSId,Code,Description,Type")] WBS wBS)
        {
            if (id != wBS.WBSId)
            {
                return NotFound();
            }

            if (wBS.Code != null && wBS.CodeExistsExcept(_context, wBS.Code, id))
            {
                ModelState.AddModelError("Code", "Este código já existe. Por favor, insira um código diferente.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wBS);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WBSExists(wBS.WBSId))
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
            return View(wBS);
        }

        // GET: WBS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wBS = await _context.WBS
                .FirstOrDefaultAsync(m => m.WBSId == id);
            if (wBS == null)
            {
                return NotFound();
            }

            return View(wBS);
        }

        // POST: WBS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wBS = await _context.WBS.FindAsync(id);
            if (wBS != null)
            {
                _context.WBS.Remove(wBS);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WBSExists(int id)
        {
            return _context.WBS.Any(e => e.WBSId == id);
        }
    }
}