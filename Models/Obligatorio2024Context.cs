using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace obligatorio2024.Models;

public partial class Obligatorio2024Context : DbContext
{
    public Obligatorio2024Context()
    {
    }

    public Obligatorio2024Context(DbContextOptions<Obligatorio2024Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Clima> Climas { get; set; }

    public virtual DbSet<Cotizacion> Cotizacions { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Mesa> Mesas { get; set; }

    public virtual DbSet<OrdenDetalle> OrdenDetalles { get; set; }

    public virtual DbSet<Ordene> Ordenes { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Reseña> Reseñas { get; set; }

    public virtual DbSet<Restaurante> Restaurantes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolPermiso> RolPermiso { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=GOHANSSJ2; Initial Catalog=obligatorio2024; Integrated Security=true; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clientes__3214EC27D1BE8A99");

            entity.HasIndex(e => e.Email, "UQ__Clientes__A9D10534DA6CAAD6").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TipoCliente)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Clima>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clima__3214EC2732B5A9A0");

            entity.ToTable("Clima");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DescripciónClima)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.ReservaId).HasColumnName("ReservaID");
            entity.Property(e => e.Temperatura).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Reserva).WithMany(p => p.Climas)
                .HasForeignKey(d => d.ReservaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Clima_Reservas");
        });

        modelBuilder.Entity<Cotizacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cotizaci__3214EC27E9D1A09F");

            entity.ToTable("Cotizacion");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cotizacion1)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("Cotizacion");
            entity.Property(e => e.Moneda)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PagosId).HasColumnName("PagosID");

            entity.HasOne(d => d.Pagos).WithMany(p => p.Cotizacions)
                .HasForeignKey(d => d.PagosId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Cotizacion_Pagos");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Menu__3214EC2734CD0CE4");

            entity.ToTable("Menu");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Categoria)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Descripción)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Disponible)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("Si");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ImagenURL");
            entity.Property(e => e.NombrePlato)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mesas__3214EC271657CC8A");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RestauranteId).HasColumnName("RestauranteID");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Mesas)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Mesas_Restaurantes");
        });

        modelBuilder.Entity<OrdenDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrdenDet__3214EC2706636105");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.OrdenId).HasColumnName("OrdenID");

            entity.HasOne(d => d.Menu).WithMany(p => p.OrdenDetalles)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("FK_OrdenDetalles_Menu");

            entity.HasOne(d => d.Orden).WithMany(p => p.OrdenDetalles)
                .HasForeignKey(d => d.OrdenId)
                .HasConstraintName("FK_OrdenDetalles_Ordenes");
        });

        modelBuilder.Entity<Ordene>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ordenes__3214EC2760D86D40");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ReservaId).HasColumnName("ReservaID");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Reserva).WithMany(p => p.Ordenes)
                .HasForeignKey(d => d.ReservaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Ordenes_Reservas");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pagos__3214EC2738E94360");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Descuento).HasColumnType("decimal(10, 4)");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Moneda)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ReservaId).HasColumnName("ReservaID");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permisos__3213E83F01B6833F");

            entity.HasIndex(e => e.NombrePermiso, "UQ__Permisos__4FC6D0ACA966DCA5").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.NombrePermiso)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre_permiso");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservas__3214EC2717DB00C4");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaReserva).HasColumnType("datetime");
            entity.Property(e => e.MesaId).HasColumnName("MesaID");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reservas_Clientes");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.MesaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reservas_Mesas");
        });

        modelBuilder.Entity<Reseña>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reseñas__3214EC27CC87EA5E");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Comentario)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.FechaReseña).HasColumnType("datetime");
            entity.Property(e => e.RestauranteId).HasColumnName("RestauranteID");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reseñas)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reseñas_Clientes");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Reseñas)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reseñas_Restaurantes");
        });

        modelBuilder.Entity<Restaurante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Restaura__3214EC2787A97561");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Dirección)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Teléfono)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC2792EFD105");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RolPermiso>(entity =>
        {
            entity.HasKey(e => new { e.IdRol, e.IdPermisos }).HasName("PK_RolPermiso");

            entity.ToTable("RolPermiso");

            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.IdPermisos).HasColumnName("idPermisos");

            entity.HasOne(d => d.Rol)
                .WithMany(p => p.RolPermiso) 
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolPermiso_Rol");

            entity.HasOne(d => d.Permiso)
                .WithMany(p => p.RolPermiso)
                .HasForeignKey(d => d.IdPermisos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolPermiso_Permiso");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC27B642DCEA");

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D1053468104A9F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Contraseña)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

