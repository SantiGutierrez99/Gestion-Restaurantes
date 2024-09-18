using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;


namespace obligatorio2024.Controllers
{
    [Authorize(Policy = "VerMesasPermiso")]
    public class MesasController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public MesasController(Obligatorio2024Context context)
        {
            _context = context;
        }

        // GET: Mesas
        public async Task<IActionResult> Index(int? restauranteId, int? numeroMesa)
        {
            var restaurantes = await _context.Restaurantes.ToListAsync();
            ViewBag.RestauranteId = new SelectList(restaurantes, "Id", "Dirección", restauranteId ?? 1);
            ViewBag.SelectedRestauranteId = restauranteId ?? 1;
            ViewBag.NumeroMesa = numeroMesa;

            var mesas = _context.Mesas.Include(m => m.Restaurante).AsQueryable();

            if (restauranteId.HasValue)
            {
                mesas = mesas.Where(m => m.RestauranteId == restauranteId.Value);
            }
            else
            {
                mesas = mesas.Where(m => m.RestauranteId == 1);
            }

            if (numeroMesa.HasValue)
            {
                mesas = mesas.Where(m => m.NumeroMesa == numeroMesa.Value);
            }

            return View(await mesas.OrderBy(m => m.NumeroMesa).ToListAsync());
        }



        // GET: Mesas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesa = await _context.Mesas
                .Include(m => m.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mesa == null)
            {
                return NotFound();
            }

            return View(mesa);
        }

        // GET: Mesas/Create
        public IActionResult Create()
        {
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id");
            return View();
        }

        // POST: Mesas/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumeroMesa,Capacidad,Estado,RestauranteId")] Mesa mesa)
        {
            if (ModelState.IsValid)
            {
                // Check if NumeroMesa already exists in the same restaurant
                bool exists = _context.Mesas.Any(m => m.RestauranteId == mesa.RestauranteId && m.NumeroMesa == mesa.NumeroMesa);
                if (exists)
                {
                    ModelState.AddModelError("NumeroMesa", "El número de mesa ya existe en este restaurante.");
                }
                else
                {
                    _context.Add(mesa);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id");
            return View(mesa);
        }

        // GET: Mesas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesa = await _context.Mesas.FindAsync(id);
            if (mesa == null)
            {
                return NotFound();
            }
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id");
            return View(mesa);
        }

        // POST: Mesas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroMesa,Capacidad,Estado,RestauranteId")] Mesa mesa)
        {
            if (id != mesa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if NumeroMesa already exists in the same restaurant
                    bool exists = _context.Mesas.Any(m => m.RestauranteId == mesa.RestauranteId && m.NumeroMesa == mesa.NumeroMesa && m.Id != mesa.Id);
                    if (exists)
                    {
                        ModelState.AddModelError("NumeroMesa", "El número de mesa ya existe en este restaurante.");
                    }
                    else
                    {
                        _context.Update(mesa);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MesaExists(mesa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id");
            return View(mesa);
        }


        // GET: Mesas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesa = await _context.Mesas
                .Include(m => m.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mesa == null)
            {
                return NotFound();
            }

            return View(mesa);
        }

        // POST: Mesas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mesa = await _context.Mesas.FindAsync(id);
            if (mesa != null)
            {
                _context.Mesas.Remove(mesa);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MesaExists(int id)
        {
            return _context.Mesas.Any(e => e.Id == id);
        }
    }
}
