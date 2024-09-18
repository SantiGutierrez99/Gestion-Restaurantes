
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using obligatorio2024.Models;
using obligatorio2024.Service;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;


using System.Threading.Tasks;

namespace obligatorio2024.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Obligatorio2024Context _context;
        private readonly WeatherService _weatherService;

        public HomeController(ILogger<HomeController> logger, Obligatorio2024Context context, WeatherService weatherService)
        {
            _logger = logger;
            _context = context;
            _weatherService = weatherService;
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = _context.Menus.ToList();

            // Obtener datos del clima
            var weatherData = await _weatherService.GetWeatherAsync("Punta del Este");

            ViewBag.MenuItems = menuItems;
            ViewBag.WeatherData = weatherData;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                var menu = new Menu
                {
                    ImagenUrl = $"/images/{fileName}",
                    NombrePlato = "Ejemplo de Plato", // Modifica según necesites
                    Precio = 100.00M // Modifica según necesites
                };

                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
