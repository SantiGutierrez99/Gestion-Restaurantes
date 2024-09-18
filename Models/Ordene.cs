using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class Ordene
{
    public int Id { get; set; }

    public int? ReservaId { get; set; }

    public decimal? Total { get; set; }

    public virtual ICollection<OrdenDetalle> OrdenDetalles { get; set; } = new List<OrdenDetalle>();

    public virtual Reserva? Reserva { get; set; }
}
