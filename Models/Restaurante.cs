using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Importa este espacio de nombres

namespace obligatorio2024.Models
{
    public partial class Restaurante
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(50, ErrorMessage = "La dirección no puede tener más de 50 caracteres.")]
        public string Dirección { get; set; } = null!;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        [StringLength(15, ErrorMessage = "El teléfono no puede tener más de 15 caracteres.")]
        public string Teléfono { get; set; } = null!;

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [StringLength(30, ErrorMessage = "La ciudad no puede tener más de 30 caracteres.")]
        public string Ciudad { get; set; } = null!;

        public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();

        public virtual ICollection<Reseña> Reseñas { get; set; } = new List<Reseña>();
    }
}
