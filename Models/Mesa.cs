using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace obligatorio2024.Models;

public partial class Mesa
{
    public int Id { get; set; }

    [Required]
    [Range(1, 60, ErrorMessage = "El número de mesa debe estar entre 1 y 60.")]
    public int NumeroMesa { get; set; }

    [Required]
    [Range(1, 20, ErrorMessage = "La capacidad de la mesa debe estar entre 1 y 20.")]
    public int Capacidad { get; set; }

    public string Estado { get; set; } = null!;

    public int? RestauranteId { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual Restaurante? Restaurante { get; set; }
}
