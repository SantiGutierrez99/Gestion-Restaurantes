using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class Clima
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public decimal Temperatura { get; set; }

    public string DescripciónClima { get; set; } = null!;

    public int? ReservaId { get; set; }

    public virtual Reserva? Reserva { get; set; }
}
