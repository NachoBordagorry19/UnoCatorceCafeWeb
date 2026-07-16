using Microsoft.EntityFrameworkCore;
using UnoCatorceCafe.Web.Dominio;

namespace UnoCatorceCafe.Web.Datos
{
    public class CafeteriaDbContext : DbContext
    {
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Producto> Productos => Set<Producto>();

        public CafeteriaDbContext(DbContextOptions<CafeteriaDbContext> opciones) 
            : base(opciones)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Categoria
            modelBuilder.Entity<Categoria>(entidad =>
            {
                entidad.ToTable("Categorias");
                entidad.HasKey(c => c.Id);
                entidad.Property(c => c.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                entidad.HasIndex(c => c.Nombre)
                    .IsUnique();
            });

            // Configuración de la entidad Producto
            modelBuilder.Entity<Producto>(entidad =>
            {
                entidad.ToTable("Productos");
                entidad.HasKey(p => p.Id);
                entidad.Property(p => p.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                entidad.Property(p => p.Precio)
                    .HasColumnType("DECIMAL(18, 2)")
                    .IsRequired();
                
                // Relación uno a muchos (una Categoria tiene muchos Productos)
                entidad.HasOne(p => p.Categoria)
                    .WithMany()
                    .HasForeignKey(p => p.CategoriaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
