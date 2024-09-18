using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;
using obligatorio2024.Service;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Obligatorio2024Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(
        new ResponseCacheAttribute
        {
            NoStore = true,
            Location = ResponseCacheLocation.None,
        }
    );
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// AUTENTICACIÓN DE USUARIO LOGEADO
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Inicio/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.AccessDeniedPath = "/Home/Index"; // ruta para acceso denegado
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VerOrdenesPermiso", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permisosClaim = context.User.FindFirst(c => c.Type == "Permisos")?.Value;
            if (permisosClaim != null)
            {
                var permisosUsuario = permisosClaim.Split(',');
                return permisosUsuario.Any(p => p.Trim().Equals("ver ordenes"));
            }
            return false;
        });
    });
    options.AddPolicy("VerClimasPermiso", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permisosClaim = context.User.FindFirst(c => c.Type == "Permisos")?.Value;
            if (permisosClaim != null)
            {
                var permisosUsuario = permisosClaim.Split(',');
                return permisosUsuario.Any(p => p.Trim().Equals("ver climas"));
            }
            return false;
        });
    });
    options.AddPolicy("VerPagosPermiso", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permisosClaim = context.User.FindFirst(c => c.Type == "Permisos")?.Value;
            if (permisosClaim != null)
            {
                var permisosUsuario = permisosClaim.Split(',');
                return permisosUsuario.Any(p => p.Trim().Equals("ver pagos"));
            }
            return false;
        });
    });
    options.AddPolicy("VerRolesPermiso", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permisosClaim = context.User.FindFirst(c => c.Type == "Permisos")?.Value;
            if (permisosClaim != null)
            {
                var permisosUsuario = permisosClaim.Split(',');
                return permisosUsuario.Any(p => p.Trim().Equals("ver roles"));
            }
            return false;
        });
    });
    options.AddPolicy("VerPermisosPermiso", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permisosClaim = context.User.FindFirst(c => c.Type == "Permisos")?.Value;
            if (permisosClaim != null)
            {
                var permisosUsuario = permisosClaim.Split(',');
                return permisosUsuario.Any(p => p.Trim().Equals("ver permisos"));
            }
            return false;
        });
    });
    options.AddPolicy("VerRestaurantesPermiso", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permisosClaim = context.User.FindFirst(c => c.Type == "Permisos")?.Value;
            if (permisosClaim != null)
            {
                var permisosUsuario = permisosClaim.Split(',');
                return permisosUsuario.Any(p => p.Trim().Equals("ver restaurantes"));
            }
            return false;
        });
    });
    options.AddPolicy("VerMesasPermiso", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permisosClaim = context.User.FindFirst(c => c.Type == "Permisos")?.Value;
            if (permisosClaim != null)
            {
                var permisosUsuario = permisosClaim.Split(',');
                return permisosUsuario.Any(p => p.Trim().Equals("ver mesas"));
            }
            return false;
        });
    });
    options.AddPolicy("VerUsuariosPermiso", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permisosClaim = context.User.FindFirst(c => c.Type == "Permisos")?.Value;
            if (permisosClaim != null)
            {
                var permisosUsuario = permisosClaim.Split(',');
                return permisosUsuario.Any(p => p.Trim().Equals("ver usuarios"));
            }
            return false;
        });
    });
    options.AddPolicy("VerClientesPermiso", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permisosClaim = context.User.FindFirst(c => c.Type == "Permisos")?.Value;
            if (permisosClaim != null)
            {
                var permisosUsuario = permisosClaim.Split(',');
                return permisosUsuario.Any(p => p.Trim().Equals("ver clientes"));
            }
            return false;
        });
    });
    options.AddPolicy("VerReservasPermiso", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permisosClaim = context.User.FindFirst(c => c.Type == "Permisos")?.Value;
            if (permisosClaim != null)
            {
                var permisosUsuario = permisosClaim.Split(',');
                return permisosUsuario.Any(p => p.Trim().Equals("ver reservas"));
            }
            return false;
        });
    });
});

// Registro de servicios
builder.Services.AddScoped<CurrencyService>();
builder.Services.AddScoped<PagoService>();
builder.Services.AddHttpClient<WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
