using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using obligatorio2024.Models;
using obligatorio2024.Service;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace obligatorio2024.Controllers
{
    public class InicioController : Controller
    {
        private readonly IUsuarioServicio _usarioServicio;

        public InicioController(IUsuarioServicio usarioServicio)
        {
            _usarioServicio = usarioServicio;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Inicio/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string contrasena)
        {
            Usuario usuario_encontrado = await _usarioServicio.GetUsuario(email, contrasena);

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            int idRol = usuario_encontrado.RolId ?? 0; // Obtener el ID del rol del usuario

            var permisosUsuario = await _usarioServicio.ObtenerPermisosPorRol(idRol); // obtiene los permisos del usuario logeado mediante un metodo

            // Crear las claims necesarias para la autenticación
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre), // claim con el nombre
                new Claim("IdRol", idRol.ToString()), // claim con el rol
                new Claim("Permisos", string.Join(",", permisosUsuario))//claim q contiene permisos
            };

            // Crear identidad de claims
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Configurar propiedades de autenticación
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            // Realizar la autenticación
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            // Guardar los permisos en la sesión
            HttpContext.Session.SetString("Permisos", string.Join(",", permisosUsuario));

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
