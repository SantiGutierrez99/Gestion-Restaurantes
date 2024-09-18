using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using obligatorio2024.Models;

namespace obligatorio2024.Controllers
{
    [Authorize(Policy = "VerOrdenesPermiso")]
    public class OrdenesController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public OrdenesController(Obligatorio2024Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var obligatorio2024Context = _context.Ordenes
                .Include(o => o.Reserva)
                .ThenInclude(r => r.Cliente)
                .Include(o => o.Reserva)
                .ThenInclude(r => r.Mesa)
                .Include(o => o.OrdenDetalles)
                .ThenInclude(od => od.Menu);

            return View(await obligatorio2024Context.ToListAsync());
        }



        // GET: Ordenes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordene = await _context.Ordenes
                .Include(o => o.OrdenDetalles)
                .ThenInclude(od => od.Menu)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ordene == null)
            {
                return NotFound();
            }

            return View(ordene);
        }


        // GET: Ordenes/Create
        public IActionResult Create()
        {
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
            return View();
        }

        // POST: Ordenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReservaId,Total")] Ordene ordene)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordene);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", ordene.ReservaId);
            return View(ordene);
        }

        // GET: Ordenes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordene = await _context.Ordenes.FindAsync(id);
            if (ordene == null)
            {
                return NotFound();
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", ordene.ReservaId);
            return View(ordene);
        }

        // POST: Ordenes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReservaId,Total")] Ordene ordene)
        {
            if (id != ordene.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordene);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdeneExists(ordene.Id))
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
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", ordene.ReservaId);
            return View(ordene);
        }



        // GET: Ordenes/AddDetail/5
        public IActionResult AddDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["OrdenId"] = id;
            ViewData["MenuId"] = new SelectList(_context.Menus, "Id", "NombrePlato");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDetail([Bind("OrdenId,MenuId,Cantidad")] OrdenDetalle ordenDetalle)
        {
            if (ModelState.IsValid)
            {
                var menu = await _context.Menus.FindAsync(ordenDetalle.MenuId);
                if (menu != null)
                {
                    var orden = await _context.Ordenes
                        .Include(o => o.OrdenDetalles)
                        .FirstOrDefaultAsync(o => o.Id == ordenDetalle.OrdenId);

                    if (orden != null)
                    {
                        var existingDetail = orden.OrdenDetalles
                            .FirstOrDefault(od => od.MenuId == ordenDetalle.MenuId);

                        if (existingDetail != null)
                        {
                            existingDetail.Cantidad += ordenDetalle.Cantidad;
                        }
                        else
                        {
                            orden.OrdenDetalles.Add(ordenDetalle);
                        }

                        orden.Total += menu.Precio * ordenDetalle.Cantidad;
                        _context.Update(orden);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Details", new { id = ordenDetalle.OrdenId });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Orden no encontrada.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Menu no encontrado.");
                }
            }
            ViewData["MenuId"] = new SelectList(_context.Menus, "Id", "NombrePlato", ordenDetalle.MenuId);
            return View(ordenDetalle);
        }


        public async Task<IActionResult> IncrementDetail(int id)
        {
            var detail = await _context.OrdenDetalles.FindAsync(id);
            if (detail != null)
            {
                var orden = await _context.Ordenes.FindAsync(detail.OrdenId);
                var menu = await _context.Menus.FindAsync(detail.MenuId);

                if (orden != null && menu != null)
                {
                    detail.Cantidad++;
                    orden.Total += menu.Precio;
                    _context.Update(detail);
                    _context.Update(orden);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Details), new { id = detail.OrdenId });
        }

        public async Task<IActionResult> DecrementDetail(int id)
        {
            var detail = await _context.OrdenDetalles.FindAsync(id);
            if (detail != null)
            {
                var orden = await _context.Ordenes.FindAsync(detail.OrdenId);
                var menu = await _context.Menus.FindAsync(detail.MenuId);

                if (orden != null && menu != null)
                {
                    if (detail.Cantidad > 1)
                    {
                        detail.Cantidad--;
                        orden.Total -= menu.Precio;
                        _context.Update(detail);
                        _context.Update(orden);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction(nameof(Details), new { id = detail.OrdenId });
        }

        public async Task<IActionResult> DeleteDetail(int id)
        {
            var detail = await _context.OrdenDetalles.FindAsync(id);
            if (detail != null)
            {
                var orden = await _context.Ordenes.FindAsync(detail.OrdenId);
                var menu = await _context.Menus.FindAsync(detail.MenuId);

                if (orden != null && menu != null)
                {
                    orden.Total -= menu.Precio * detail.Cantidad;
                    _context.OrdenDetalles.Remove(detail);
                    _context.Update(orden);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Details), new { id = detail.OrdenId });
        }



        // GET: Ordenes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordene = await _context.Ordenes
                .Include(o => o.Reserva)
                .ThenInclude(r => r.Cliente)
                .Include(o => o.Reserva)
                .ThenInclude(r => r.Mesa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordene == null)
            {
                return NotFound();
            }

            return View(ordene);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordene = await _context.Ordenes
                .Include(o => o.OrdenDetalles)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ordene != null)
            {
                _context.Ordenes.Remove(ordene);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrdeneExists(int id)
        {
            return _context.Ordenes.Any(e => e.Id == id);
        }



    }
}
