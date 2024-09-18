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
    public class RolPermisoesController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public RolPermisoesController(Obligatorio2024Context context)
        {
            _context = context;
        }

        // GET: RolPermisoes
        public async Task<IActionResult> Index()
        {
            var obligatorio2024Context = _context.RolPermiso.Include(r => r.Permiso).Include(r => r.Rol);
            return View(await obligatorio2024Context.ToListAsync());
        }

        // GET: RolPermisoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolPermiso = await _context.RolPermiso
                .Include(r => r.Permiso)
                .Include(r => r.Rol)
                .FirstOrDefaultAsync(m => m.IdRol == id);
            if (rolPermiso == null)
            {
                return NotFound();
            }

            return View(rolPermiso);
        }

        // GET: RolPermisoes/Create
        public IActionResult Create()
        {
            ViewData["IdPermisos"] = new SelectList(_context.Permisos, "Id", "Id");
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
        }

        // POST: RolPermisoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRol,IdPermisos")] RolPermiso rolPermiso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rolPermiso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPermisos"] = new SelectList(_context.Permisos, "Id", "Id", rolPermiso.IdPermisos);
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Id", rolPermiso.IdRol);
            return View(rolPermiso);
        }

        // GET: RolPermisoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolPermiso = await _context.RolPermiso.FindAsync(id);
            if (rolPermiso == null)
            {
                return NotFound();
            }
            ViewData["IdPermisos"] = new SelectList(_context.Permisos, "Id", "Id", rolPermiso.IdPermisos);
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Id", rolPermiso.IdRol);
            return View(rolPermiso);
        }

        // POST: RolPermisoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRol,IdPermisos")] RolPermiso rolPermiso)
        {
            if (id != rolPermiso.IdRol)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rolPermiso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolPermisoExists(rolPermiso.IdRol))
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
            ViewData["IdPermisos"] = new SelectList(_context.Permisos, "Id", "Id", rolPermiso.IdPermisos);
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Id", rolPermiso.IdRol);
            return View(rolPermiso);
        }

        // GET: RolPermisoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolPermiso = await _context.RolPermiso
                .Include(r => r.Permiso)
                .Include(r => r.Rol)
                .FirstOrDefaultAsync(m => m.IdRol == id);
            if (rolPermiso == null)
            {
                return NotFound();
            }

            return View(rolPermiso);
        }

        // POST: RolPermisoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rolPermiso = await _context.RolPermiso.FindAsync(id);
            if (rolPermiso != null)
            {
                _context.RolPermiso.Remove(rolPermiso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RolPermisoExists(int id)
        {
            return _context.RolPermiso.Any(e => e.IdRol == id);
        }
    }
}
