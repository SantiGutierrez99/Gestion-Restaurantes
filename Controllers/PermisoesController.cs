using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;

namespace obligatorio2024.Controllers
{
    [Authorize(Policy = "VerPermisosPermiso")]
    public class PermisoesController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public PermisoesController(Obligatorio2024Context context)
        {
            _context = context;
        }

        // GET: Permisoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Permisos.ToListAsync());
        }

        // GET: Permisoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permiso == null)
            {
                return NotFound();
            }

            return View(permiso);
        }

        // GET: Permisoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Permisoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombrePermiso,Descripcion,PermisosSeleccionados")] Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                // Añadir el permiso a la base de datos
                _context.Add(permiso);
                await _context.SaveChangesAsync();

                // Asignar permisos a roles seleccionados (si se aplica)
                var rolesSeleccionados = Request.Form["rolesSeleccionados"].ToString().Split(',').Select(int.Parse).ToList();
                if (rolesSeleccionados.Any())
                {
                    foreach (var rolId in rolesSeleccionados)
                    {
                        var rolPermiso = new RolPermiso
                        {
                            IdRol = rolId,
                            IdPermisos = permiso.Id
                        };
                        _context.Add(rolPermiso);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Obtener la lista de roles para asignar al permiso (si se aplica)
            var roles = await _context.Roles.ToListAsync();
            ViewBag.Roles = roles;

            return View(permiso);
        }



        // GET: Permisoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null)
            {
                return NotFound();
            }
            return View(permiso);
        }

        // POST: Permisoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombrePermiso,Descripcion")] Permiso permiso)
        {
            if (id != permiso.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permiso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermisoExists(permiso.Id))
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
            return View(permiso);
        }

        // GET: Permisoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permiso == null)
            {
                return NotFound();
            }

            return View(permiso);
        }

        // POST: Permisoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso != null)
            {
                _context.Permisos.Remove(permiso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermisoExists(int id)
        {
            return _context.Permisos.Any(e => e.Id == id);
        }
    }
}
