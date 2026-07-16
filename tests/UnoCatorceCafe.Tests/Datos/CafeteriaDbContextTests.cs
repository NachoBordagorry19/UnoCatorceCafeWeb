using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UnoCatorceCafe.Web.Dominio;
using UnoCatorceCafe.Web.Datos;

namespace UnoCatorceCafe.Tests.Datos
{
    public class CafeteriaDbContextTests : IDisposable
    {
        private readonly SqliteConnection _conexion;
        private readonly DbContextOptions<CafeteriaDbContext> _opciones;

        public CafeteriaDbContextTests()
        {
            // Crear una conexión SQLite en memoria y abrirla para mantener viva la base de datos durante el test
            _conexion = new SqliteConnection("Filename=:memory:");
            _conexion.Open();

            _opciones = new DbContextOptionsBuilder<CafeteriaDbContext>()
                .UseSqlite(_conexion)
                .Options;

            // Asegurar que la estructura de tablas esté creada
            using var contexto = new CafeteriaDbContext(_opciones);
            contexto.Database.EnsureCreated();
        }

        [Fact]
        public void GuardarCategoria_DebePersistirEnBaseDeDatos()
        {
            // Arrange
            var categoria = new Categoria(Guid.NewGuid(), "Bebidas Calientes", 1, true);

            // Act
            using (var contextoEscritura = new CafeteriaDbContext(_opciones))
            {
                contextoEscritura.Categorias.Add(categoria);
                contextoEscritura.SaveChanges();
            }

            // Assert
            using (var contextoLectura = new CafeteriaDbContext(_opciones))
            {
                var categoriaPersistida = contextoLectura.Categorias.Find(categoria.Id);
                Assert.NotNull(categoriaPersistida);
                Assert.Equal(categoria.Nombre, categoriaPersistida.Nombre);
                Assert.Equal(categoria.OrdenVisualizacion, categoriaPersistida.OrdenVisualizacion);
                Assert.Equal(categoria.EstaActiva, categoriaPersistida.EstaActiva);
            }
        }

        [Fact]
        public async Task GuardarProducto_ConCategoriaValida_DebePersistirEnBaseDeDatos()
        {
            // Arrange
            var categoria = new Categoria(Guid.NewGuid(), "Bebidas", 1, true);
            var producto = new Producto(
                Guid.NewGuid(), 
                categoria.Id, 
                "Latte", 
                "Café con leche", 
                140m, 
                "latte.png", 
                true, 
                1
            );

            // Act
            using (var contextoEscritura = new CafeteriaDbContext(_opciones))
            {
                contextoEscritura.Categorias.Add(categoria);
                contextoEscritura.Productos.Add(producto);
                await contextoEscritura.SaveChangesAsync();
            }

            // Assert
            using (var contextoLectura = new CafeteriaDbContext(_opciones))
            {
                var productoPersistido = await contextoLectura.Productos
                    .Include(p => p.Categoria)
                    .FirstOrDefaultAsync(p => p.Id == producto.Id);

                Assert.NotNull(productoPersistido);
                Assert.Equal(producto.Nombre, productoPersistido.Nombre);
                Assert.Equal(producto.Precio, productoPersistido.Precio);
                Assert.NotNull(productoPersistido.Categoria);
                Assert.Equal(categoria.Nombre, productoPersistido.Categoria.Nombre);
            }
        }

        [Fact]
        public void GuardarProducto_ConCategoriaInexistente_DebeLanzarExcepcion()
        {
            // Arrange
            var productoSinCategoriaReal = new Producto(
                Guid.NewGuid(), 
                Guid.NewGuid(), // ID de categoría aleatorio inexistente
                "Filtro del día", 
                "Café filtrado", 
                100m, 
                "", 
                true, 
                1
            );

            // Act & Assert
            using var contextoEscritura = new CafeteriaDbContext(_opciones);
            contextoEscritura.Productos.Add(productoSinCategoriaReal);

            // En SQLite con EnsureCreated(), las claves foráneas están habilitadas.
            // Guardar un registro sin su clave foránea válida debe fallar.
            Assert.Throws<DbUpdateException>(() => contextoEscritura.SaveChanges());
        }

        public void Dispose()
        {
            _conexion.Dispose();
        }
    }
}
