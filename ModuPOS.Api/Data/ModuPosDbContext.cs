using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ModuPOS.Api.Entities;
using ModuPOS.Shared.Enums;

namespace ModuPOS.Api.Data
{
    public class ModuPosDbContext : DbContext
    {
        public ModuPosDbContext(DbContextOptions<ModuPosDbContext> options) : base(options) { }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();
        public DbSet<Venta> Ventas => Set<Venta>();
        public DbSet<VentaDetalle> VentaDetalles => Set<VentaDetalle>();

        //soft delete
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var deletedEntries = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Deleted);

            foreach (EntityEntry<BaseEntity> entry in deletedEntries)
            {
                entry.State = EntityState.Modified;

                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        //configuracion de modelos y relaciones
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<MetodoPago>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Venta>().HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Producto>(e =>
            {
                e.HasIndex(p => p.SKU).IsUnique();
                e.Property(p => p.SKU).HasMaxLength(50).IsRequired();
                e.Property(p => p.Nombre).HasMaxLength(100).IsRequired();

                e.Property(p => p.PrecioActual).HasColumnType("decimal(18,4)");
                e.Property(p => p.Stock).HasDefaultValue(0);
            });

            modelBuilder.Entity<MetodoPago>(e =>
            {
                e.Property(m => m.Nombre).HasMaxLength(40).IsRequired();
            });

            modelBuilder.Entity<Venta>(e =>
            {
                e.HasIndex(v => v.Folio).IsUnique();
                e.Property(v => v.Folio).HasMaxLength(20).IsRequired();
                e.Property(v => v.Fecha).HasDefaultValueSql("GETDATE()");
                e.Property(v => v.Subtotal).HasColumnType("decimal(18,4)");
                e.Property(v => v.Impuestos).HasColumnType("decimal(18,4)");
                e.Property(v => v.DescuentoTotal).HasColumnType("decimal(18,4)");

                // columna calculada, leída mas no almacenada
                e.Property(v => v.Total)
                    .HasColumnType("decimal(18,4)")
                    .HasComputedColumnSql("(Subtotal + Impuestos - DescuentoTotal)", stored: false);

                e.Property(v => v.Estado)
                    .HasConversion<string>()
                    .HasMaxLength(40)
                    .HasDefaultValue(EstadoVenta.Completada);

                e.HasOne(v => v.MetodoPago)
                    .WithMany(m => m.Ventas)
                    .HasForeignKey(v => v.MetodoPagoId)
                    .OnDelete(DeleteBehavior.Restrict); // no borrar método si tiene ventas
            });

            modelBuilder.Entity<VentaDetalle>(e =>
            {
                e.Property(d => d.PrecioUnitarioHistorico).HasColumnType("decimal(18,4)");

                e.Property(d => d.SubtotalLinea)
                 .HasColumnType("decimal(18,4)")
                 .HasComputedColumnSql("(Cantidad * PrecioUnitarioHistorico)", stored: false);

                e.HasOne(d => d.Venta)
                 .WithMany(v => v.Detalles)
                 .HasForeignKey(d => d.VentaId)
                 .OnDelete(DeleteBehavior.Cascade); // borrar venta y sus detalles

                e.HasOne(d => d.Producto)
                 .WithMany()
                 .HasForeignKey(d => d.ProductoId)
                 .OnDelete(DeleteBehavior.Restrict); // no borrar producto si tiene detalles
            });
        }
    }
}
