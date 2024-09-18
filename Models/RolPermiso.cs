namespace obligatorio2024.Models
{
    public class RolPermiso
    {
        public int IdRol { get; set; }
        public int IdPermisos { get; set; }

        public virtual Role Rol { get; set; }
        public virtual Permiso Permiso { get; set; }
    }



}