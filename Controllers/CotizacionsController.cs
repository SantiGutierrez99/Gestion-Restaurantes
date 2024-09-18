using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;

namespace obligatorio2024.Controllers
{
    public class CotizacionsController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public CotizacionsController(Obligatorio2024Context context)
        {
            _context = context;
        }

        // GET: Cotizacions
        public async Task<IActionResult> Index()
        {
            var obligatorio2024Context = _context.Cotizacions.Include(c => c.Pagos);
            return View(await obligatorio2024Context.ToListAsync());
        }

        // GET: Cotizacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotizacion = await _context.Cotizacions
                .Include(c => c.Pagos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cotizacion == null)
            {
                return NotFound();
            }

            return View(cotizacion);
        }

        // GET: Cotizacions/Create
        public IActionResult Create()
        {
            ViewData["PagosId"] = new SelectList(_context.Pagos, "Id", "Id");
            return View();
        }

        // POST: Cotizacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Cotizacion1,PagosId")] Cotizacion cotizacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cotizacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PagosId"] = new SelectList(_context.Pagos, "Id", "Id", cotizacion.PagosId);
            return View(cotizacion);
        }

        // GET: Cotizacions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotizacion = await _context.Cotizacions.FindAsync(id);
            if (cotizacion == null)
            {
                return NotFound();
            }
            ViewData["PagosId"] = new SelectList(_context.Pagos, "Id", "Id", cotizacion.PagosId);
            return View(cotizacion);
        }

        // POST: Cotizacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cotizacion1,PagosId")] Cotizacion cotizacion)
        {
            if (id != cotizacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cotizacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CotizacionExists(cotizacion.Id))
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
            ViewData["PagosId"] = new SelectList(_context.Pagos, "Id", "Id", cotizacion.PagosId);
            return View(cotizacion);
        }

        // GET: Cotizacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotizacion = await _context.Cotizacions
                .Include(c => c.Pagos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cotizacion == null)
            {
                return NotFound();
            }

            return View(cotizacion);
        }

        // POST: Cotizacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cotizacion = await _context.Cotizacions.FindAsync(id);
            if (cotizacion != null)
            {
                _context.Cotizacions.Remove(cotizacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CotizacionExists(int id)
        {
            return _context.Cotizacions.Any(e => e.Id == id);
        }
    }
}
