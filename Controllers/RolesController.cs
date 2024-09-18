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
    [Authorize(Policy = "VerRolesPermiso")]
    public class RolesController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public RolesController(Obligatorio2024Context context)
        {
            _context = context;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Roles/Create

        // GET: Roles/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Obtener la lista de permisos
            var permisos = await _context.Permisos.ToListAsync();
            ViewBag.Permisos = permisos;

            return View(new Role());
        }
        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Role role, int[] PermisosSeleccionados)
        {
            if (ModelState.IsValid)
            {
                // Añadir el rol a la base de datos
                _context.Add(role);
                await _context.SaveChangesAsync();

                // Asignar permisos al rol
                if (PermisosSeleccionados != null && PermisosSeleccionados.Any())
                {
                    foreach (var permisoId in PermisosSeleccionados)
                    {
                        var rolPermiso = new RolPermiso
                        {
                            IdRol = role.Id,
                            IdPermisos = permisoId
                        };
                        _context.Add(rolPermiso);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Obtener la lista de permisos nuevamente en caso de error
            var permisos = await _context.Permisos.ToListAsync();
            ViewBag.Permisos = permisos;
            return View(role);
        }




        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .Include(r => r.RolPermiso)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            var allPermisos = await _context.Permisos.ToListAsync();
            var permisosSeleccionados = role.RolPermiso.Select(rp => rp.IdPermisos).ToList();

            ViewBag.Permisos = allPermisos;
            ViewBag.PermisosSeleccionados = permisosSeleccionados;

            return View(role);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Role role, int[] PermisosSeleccionados)
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();

                    // Actualizar permisos
                    var existingPermissions = _context.RolPermiso.Where(rp => rp.IdRol == id);
                    _context.RolPermiso.RemoveRange(existingPermissions);

                    foreach (var permisoId in PermisosSeleccionados)
                    {
                        _context.RolPermiso.Add(new RolPermiso { IdRol = id, IdPermisos = permisoId });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.Id))
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

            var allPermisos = await _context.Permisos.ToListAsync();
            var permisosSeleccionados = role.RolPermiso.Select(rp => rp.IdPermisos).ToList();

            ViewBag.Permisos = allPermisos;
            ViewBag.PermisosSeleccionados = permisosSeleccionados;

            return View(role);
        }
    

    // GET: Roles/Delete/5
    public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
