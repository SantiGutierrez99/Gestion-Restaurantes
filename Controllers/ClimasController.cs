using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;

namespace obligatorio2024.Controllers
{
    [Authorize(Policy = "VerClimasPermiso")]
    public class ClimasController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public ClimasController(Obligatorio2024Context context)
        {
            _context = context;
        }

        // GET: Climas
        public async Task<IActionResult> Index(int? reservaId)
        {
            var climas = from c in _context.Climas
                         select c;

            if (reservaId.HasValue)
            {
                climas = climas.Where(c => c.ReservaId == reservaId.Value);
            }

            return View(await climas.ToListAsync());
        }

        // GET: Climas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clima = await _context.Climas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clima == null)
            {
                return NotFound();
            }

            return View(clima);
        }

        // GET: Climas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Climas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fecha,Temperatura,DescripciónClima,ReservaId")] Clima clima)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clima);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clima);
        }

        // GET: Climas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clima = await _context.Climas.FindAsync(id);
            if (clima == null)
            {
                return NotFound();
            }
            return View(clima);
        }

        // POST: Climas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,Temperatura,DescripciónClima,ReservaId")] Clima clima)
        {
            if (id != clima.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clima);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClimaExists(clima.Id))
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
            return View(clima);
        }

        // GET: Climas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clima = await _context.Climas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clima == null)
            {
                return NotFound();
            }

            return View(clima);
        }

        // POST: Climas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clima = await _context.Climas.FindAsync(id);
            if (clima != null)
            {
                _context.Climas.Remove(clima);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClimaExists(int id)
        {
            return _context.Climas.Any(e => e.Id == id);
        }
    }
}
