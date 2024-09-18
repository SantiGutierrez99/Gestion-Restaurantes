using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class OrdenDetalle
{
    public int Id { get; set; }

    public int OrdenId { get; set; }

    public int MenuId { get; set; }

    public int Cantidad { get; set; }

    public virtual Menu? Menu { get; set; } 

    public virtual Ordene? Orden { get; set; } 
}
