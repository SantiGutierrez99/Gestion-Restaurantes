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
    public class MenusController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public MenusController(Obligatorio2024Context context)
        {
            _context = context;
        }

        // GET: Menus
        public async Task<IActionResult> Index()
        {
            return View(await _context.Menus.ToListAsync());
        }

        // GET: Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Menus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombrePlato,Descripción,Precio,ImagenUrl,Categoria,Disponible")] Menu menu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(menu);
        }

        // GET: Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            return View(menu);
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombrePlato,Descripción,Precio,ImagenUrl,Categoria,Disponible")] Menu menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
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
            return View(menu);
        }

        // GET: Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Entradas()
        {
            var category = "Entradas";
            var menus = await _context.Menus.Where(m => m.Categoria == category).ToListAsync();
            ViewData["Category"] = category;
            return View("MenuCategoria", menus);
        }

        // Acción para Platos Principales
        public async Task<IActionResult> PlatosPrincipales()
        {
            var category = "Platos Principales";
            var menus = await _context.Menus.Where(m => m.Categoria == category).ToListAsync();
            ViewData["Category"] = category;
            return View("MenuCategoria", menus);
        }

        // Acción para Guarniciones
        public async Task<IActionResult> Guarniciones()
        {
            var category = "Guarniciones";
            var menus = await _context.Menus.Where(m => m.Categoria == category).ToListAsync();
            ViewData["Category"] = category;
            return View("MenuCategoria", menus);
        }

        // Acción para Postres
        public async Task<IActionResult> Postres()
        {
            var category = "Postres";
            var menus = await _context.Menus.Where(m => m.Categoria == category).ToListAsync();
            ViewData["Category"] = category;
            return View("MenuCategoria", menus);
        }

        // Acción para Menús Especiales
        public async Task<IActionResult> MenusEspeciales()
        {
            var category = "Menús Especiales";
            var menus = await _context.Menus.Where(m => m.Categoria == category).ToListAsync();
            ViewData["Category"] = category;
            return View("MenuCategoria", menus);
        }

        // Acción para Bebidas
        public async Task<IActionResult> Bebidas()
        {
            var category = "Bebidas";
            var menus = await _context.Menus.Where(m => m.Categoria == category).ToListAsync();
            ViewData["Category"] = category;
            return View("MenuCategoria", menus);
        }

        // Acción para Bebidas Alcohólicas
        public async Task<IActionResult> BebidasAlcoholicas()
        {
            var category = "Bebidas Alcohólicas";
            var menus = await _context.Menus.Where(m => m.Categoria == category).ToListAsync();
            ViewData["Category"] = category;
            return View("MenuCategoria", menus);
        }

        // Acción para Brunch
        public async Task<IActionResult> Brunch()
        {
            var category = "Brunch";
            var menus = await _context.Menus.Where(m => m.Categoria == category).ToListAsync();
            ViewData["Category"] = category;
            return View("MenuCategoria", menus);
        }
    }
}
