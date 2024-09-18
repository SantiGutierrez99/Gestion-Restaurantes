using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class Permiso
{
    public int Id { get; set; }

    public string NombrePermiso { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Role> IdRols { get; set; } = new List<Role>();
    public virtual ICollection<RolPermiso> RolPermiso { get; set; } = new List<RolPermiso>();
}
