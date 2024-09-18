using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;
using obligatorio2024.Service;

namespace obligatorio2024.Controllers
{
    [Authorize(Policy = "VerPagosPermiso")]
    public class PagoesController : Controller
    {
        private readonly Obligatorio2024Context _context;
        private readonly PagoService _pagoService;
        private readonly WeatherService _weatherService;

        public PagoesController(Obligatorio2024Context context, PagoService pagoService, WeatherService weatherService)
        {
            _context = context;
            _pagoService = pagoService;
            _weatherService = weatherService;
        }

        // GET: Pagoes
        public async Task<IActionResult> Index(int? restauranteId)
        {
            var restaurantes = await _context.Restaurantes.ToListAsync();
            ViewBag.RestauranteId = new SelectList(restaurantes, "Id", "Dirección", restauranteId ?? 1);
            ViewBag.SelectedRestauranteId = restauranteId ?? 1;

            var pagos = _context.Pagos
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Cliente)
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Mesa)
                    .ThenInclude(m => m.Restaurante)
                .AsQueryable();

            if (restauranteId.HasValue)
            {
                pagos = pagos.Where(p => p.Reserva.Mesa.RestauranteId == restauranteId.Value);
            }
            else
            {
                pagos = pagos.Where(p => p.Reserva.Mesa.RestauranteId == 1);
            }

            return View(await pagos.ToListAsync());
        }

        // GET: Pagoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // GET: Pagoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pagoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservaId,Monto,MetodoPago,Moneda,Descuento")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var reserva = await _context.Reservas
                        .Include(r => r.Mesa)
                        .ThenInclude(m => m.Restaurante)
                        .Include(r => r.Cliente)
                        .FirstOrDefaultAsync(r => r.Id == pago.ReservaId);

                    if (reserva == null)
                    {
                        ModelState.AddModelError("ReservaId", "La reserva no existe.");
                        return View(pago);
                    }

                    var existingPago = await _context.Pagos
                        .FirstOrDefaultAsync(p => p.ReservaId == pago.ReservaId && p.Id != pago.Id);

                    if (existingPago != null)
                    {
                        ModelState.AddModelError("ReservaId", "Ya existe un pago para esta reserva.");
                        return View(pago);
                    }

                    var (nuevoPago, cotizacion) = await _pagoService.CrearPagoAsync(pago.Monto, pago.Moneda, pago.MetodoPago);
                    nuevoPago.ReservaId = pago.ReservaId;

                    decimal descuentoCliente = 0;
                    decimal descuentoClima = 0;

                    // Aplicar descuento del cliente
                    if (reserva.Cliente != null)
                    {
                        var porcentajeDescuentoCliente = ObtenerPorcentajeDescuentoCliente(reserva.Cliente.TipoCliente);
                        descuentoCliente = nuevoPago.Monto * porcentajeDescuentoCliente;
                    }

                    // Crear la entrada de clima y aplicar descuentos basados en el clima
                    var restaurante = await _context.Restaurantes.FirstOrDefaultAsync(r => r.Id == reserva.Mesa.RestauranteId);
                    if (restaurante != null)
                    {
                        var clima = await CreateClimaEntryAsync(reserva.FechaReserva, reserva.Id, restaurante.Ciudad);
                        var porcentajeDescuentoClima = ObtenerPorcentajeDescuentoClima(clima);
                        descuentoClima = nuevoPago.Monto * porcentajeDescuentoClima;
                    }

                    nuevoPago.Descuento = (decimal?)(descuentoCliente + descuentoClima); // Conversión explícita
                    nuevoPago.Monto -= nuevoPago.Descuento ?? 0; // Resta el descuento total

                    _context.Add(nuevoPago);
                    await _context.SaveChangesAsync();

                    cotizacion.PagosId = nuevoPago.Id; // Asignamos el ID del pago después de guardar el pago
                    _context.Add(cotizacion);

                    // Actualizar el estado de la mesa a "Disponible"
                    if (reserva.Mesa != null)
                    {
                        reserva.Mesa.Estado = "Disponible";
                        _context.Update(reserva.Mesa);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(pago);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error al procesar el pago.");
                    Console.WriteLine(ex);
                    return View(pago);
                }
            }
            return View(pago);
        }

        [HttpGet]
        public async Task<IActionResult> GetMonto(int reservaId)
        {
            var orden = await _context.Ordenes.FirstOrDefaultAsync(o => o.ReservaId == reservaId);
            if (orden != null)
            {
                return Json(new { success = true, monto = orden.Total });
            }
            else
            {
                return Json(new { success = false, message = "No existe una orden para calcular el monto" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDescuento(int reservaId)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .ThenInclude(m => m.Restaurante)
                .FirstOrDefaultAsync(r => r.Id == reservaId);

            if (reserva != null && reserva.Cliente != null && reserva.Mesa?.Restaurante != null)
            {
                var porcentajeDescuentoCliente = ObtenerPorcentajeDescuentoCliente(reserva.Cliente.TipoCliente);
                var porcentajeDescuentoClima = 0m;

                var clima = await _context.Climas.FirstOrDefaultAsync(c => c.ReservaId == reservaId);
                if (clima != null)
                {
                    porcentajeDescuentoClima = ObtenerPorcentajeDescuentoClima(clima);
                }

                var descuentoTotal = (porcentajeDescuentoCliente + porcentajeDescuentoClima) * 100; // Convertir a porcentaje

                return Json(new { success = true, descuento = descuentoTotal });
            }

            return Json(new { success = false, message = "No se pudo calcular el descuento" });
        }

        // GET: Pagoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        // POST: Pagoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Monto,MetodoPago,Moneda,Descuento")] Pago pago)
        {
            if (id != pago.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var pagoOriginal = await _context.Pagos.FindAsync(id);
                    if (pagoOriginal == null)
                    {
                        return NotFound();
                    }

                    var (nuevoPago, cotizacion) = await _pagoService.CrearPagoAsync(pago.Monto, pago.Moneda, pago.MetodoPago);

                    pagoOriginal.Monto = nuevoPago.Monto;
                    pagoOriginal.MetodoPago = nuevoPago.MetodoPago;
                    pagoOriginal.Moneda = nuevoPago.Moneda;

                    var reserva = await _context.Reservas
                        .Include(r => r.Mesa)
                        .ThenInclude(m => m.Restaurante)
                        .Include(r => r.Cliente)
                        .FirstOrDefaultAsync(r => r.Id == pagoOriginal.ReservaId);

                    if (reserva != null)
                    {
                        decimal descuentoTotal = 0;

                        // Aplicar descuento del cliente
                        if (reserva.Cliente != null)
                        {
                            var porcentajeDescuentoCliente = ObtenerPorcentajeDescuentoCliente(reserva.Cliente.TipoCliente);
                            var descuentoCliente = pagoOriginal.Monto * porcentajeDescuentoCliente;
                            descuentoTotal += descuentoCliente;
                            pagoOriginal.Monto -= descuentoCliente;
                        }

                        // Aplicar descuento del clima
                        if (reserva.Mesa?.Restaurante != null)
                        {
                            var clima = await CreateClimaEntryAsync(reserva.FechaReserva, reserva.Id, reserva.Mesa.Restaurante.Ciudad);
                            var porcentajeDescuentoClima = ObtenerPorcentajeDescuentoClima(clima);
                            var descuentoClima = pagoOriginal.Monto * porcentajeDescuentoClima;
                            descuentoTotal += descuentoClima;
                            pagoOriginal.Monto -= descuentoClima;
                        }

                        pagoOriginal.Descuento = (decimal?)descuentoTotal; // Conversión explícita
                    }

                    _context.Update(pagoOriginal);
                    await _context.SaveChangesAsync();

                    cotizacion.PagosId = pagoOriginal.Id; // Asignamos el ID del pago después de guardar el pago
                    _context.Add(cotizacion);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pago.Id))
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
            return View(pago);
        }

        // GET: Pagoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago != null)
            {
                _context.Pagos.Remove(pago);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }

        private decimal ObtenerPorcentajeDescuentoCliente(string tipoCliente)
        {
            decimal porcentajeDescuento = 0;
            switch (tipoCliente)
            {
                case "Frecuente":
                    porcentajeDescuento = 0.10m;
                    break;
                case "VIP":
                    porcentajeDescuento = 0.20m;
                    break;
            }
            return porcentajeDescuento;
        }

        private decimal ObtenerPorcentajeDescuentoClima(Clima clima)
        {
            decimal porcentajeDescuentoClima = 0;

            if (clima.Temperatura < 0)
            {
                porcentajeDescuentoClima = 0.10m;
            }
            else if (clima.Temperatura < 10)
            {
                porcentajeDescuentoClima = 0.05m;
            }

            if (clima.DescripciónClima.Contains("rain", StringComparison.OrdinalIgnoreCase) ||
                clima.DescripciónClima.Contains("lluvia", StringComparison.OrdinalIgnoreCase))
            {
                porcentajeDescuentoClima += 0.05m;
            }

            return porcentajeDescuentoClima;
        }

        private async Task<Clima> CreateClimaEntryAsync(DateTime fechaReserva, int reservaId, string ciudad)
        {
            var weather = await _weatherService.GetWeatherAsync(ciudad);

            var clima = new Clima
            {
                Fecha = fechaReserva,
                Temperatura = (decimal)weather.Main.Temp.Value, // Conversión explícita de double a decimal
                DescripciónClima = weather.Weather.First().Description,
                ReservaId = reservaId
            };

            _context.Climas.Add(clima);
            await _context.SaveChangesAsync();

            return clima;
        }
    }
}
