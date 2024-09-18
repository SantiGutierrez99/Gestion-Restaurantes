using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class Menu
{
    public int Id { get; set; }

    public string NombrePlato { get; set; } = null!;

    public string Descripción { get; set; } = null!;

    public decimal Precio { get; set; }

    public string? ImagenUrl { get; set; }

    public string Categoria { get; set; } = null!;

    public string Disponible { get; set; } = null!;

    public virtual ICollection<OrdenDetalle> OrdenDetalles { get; set; } = new List<OrdenDetalle>();
}
