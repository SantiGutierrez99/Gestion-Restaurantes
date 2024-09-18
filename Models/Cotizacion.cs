using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class Cotizacion
{
    public int Id { get; set; }

    public decimal Cotizacion1 { get; set; }

    public int? PagosId { get; set; }

    public string Moneda { get; set; } = null!;

    public virtual Pago? Pagos { get; set; }
}
