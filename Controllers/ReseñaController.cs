using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;

namespace obligatorio2024.Controllers
{
    public class ReseñaController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public ReseñaController(Obligatorio2024Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? restauranteId)
        {
            var restaurantes = await _context.Restaurantes.ToListAsync();
            ViewBag.RestauranteId = new SelectList(restaurantes, "Id", "Nombre", restauranteId);
            ViewBag.SelectedRestauranteId = restauranteId;

            var reseñas = _context.Reseñas
                .Include(r => r.Cliente)
                .Include(r => r.Restaurante)
                .AsQueryable();

            if (restauranteId.HasValue)
            {
                reseñas = reseñas.Where(r => r.RestauranteId == restauranteId.Value);
            }


            return View(await reseñas.ToListAsync());
        }

        // GET: Reseña/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reseña = await _context.Reseñas
                .Include(r => r.Cliente)
                .Include(r => r.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reseña == null)
            {
                return NotFound();
            }

            return View(reseña);
        }
        // GET: Reseña/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre");
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,RestauranteId,Puntaje,Comentario,FechaReseña")] Reseña reseña)
        {
            if (ModelState.IsValid)
            {
                reseña.FechaReseña = DateTime.Now;
                _context.Add(reseña);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", reseña.ClienteId);
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Nombre", reseña.RestauranteId);
            return View(reseña);
        }


        // GET: Reseña/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reseña = await _context.Reseñas.FindAsync(id);
            if (reseña == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", reseña.ClienteId);
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Nombre", reseña.RestauranteId);
            return View(reseña);
        }

        // POST: Reseña/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,RestauranteId,Puntaje,Comentario,FechaReseña")] Reseña reseña)
        {
            if (id != reseña.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reseña);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReseñaExists(reseña.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", reseña.ClienteId);
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Nombre", reseña.RestauranteId);
            return View(reseña);
        }

        // GET: Reseña/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reseña = await _context.Reseñas
                .Include(r => r.Cliente)
                .Include(r => r.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reseña == null)
            {
                return NotFound();
            }

            return View(reseña);
        }

        // POST: Reseña/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reseña = await _context.Reseñas.FindAsync(id);
            if (reseña != null)
            {
                _context.Reseñas.Remove(reseña);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReseñaExists(int id)
        {
            return _context.Reseñas.Any(e => e.Id == id);
        }
    }
}
