using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Importa este espacio de nombres

namespace obligatorio2024.Models
{
    public partial class Role
    {
        public int Id { get; set; }

        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        public string Nombre { get; set; } = null!;

        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        public virtual ICollection<Permiso> IdPermisos { get; set; } = new List<Permiso>();

        public virtual ICollection<RolPermiso> RolPermiso { get; set; } = new List<RolPermiso>();
    }
}
