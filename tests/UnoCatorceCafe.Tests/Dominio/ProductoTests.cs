using System;
using Xunit;
using UnoCatorceCafe.Web.Dominio;

namespace UnoCatorceCafe.Tests.Dominio
{
    public class ProductoTests
    {
        [Fact]
        public void CrearProducto_ConDatosValidos_DebeCrearInstancia()
        {
            // Arrange
            var id = Guid.NewGuid();
            var categoriaId = Guid.NewGuid();
            var nombre = "Flat White";
            var descripcion = "Doble shot de espresso con leche emulsionada";
            var precio = 160m;
            var urlImagen = "flat-white.jpg";
            var disponible = true;
            var orden = 1;

            // Act
            var producto = new Producto(id, categoriaId, nombre, descripcion, precio, urlImagen, disponible, orden);

            // Assert
            Assert.Equal(id, producto.Id);
            Assert.Equal(categoriaId, producto.CategoriaId);
            Assert.Equal(nombre, producto.Nombre);
            Assert.Equal(descripcion, producto.Descripcion);
            Assert.Equal(precio, producto.Precio);
            Assert.Equal(urlImagen, producto.UrlImagen);
            Assert.Equal(disponible, producto.EstaDisponible);
            Assert.Equal(orden, producto.OrdenVisualizacion);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CrearProducto_ConNombreInvalido_DebeLanzarArgumentException(string? nombreInvalido)
        {
            // Arrange
            var id = Guid.NewGuid();
            var categoriaId = Guid.NewGuid();
            var descripcion = "Café";
            var precio = 150m;
            var urlImagen = "foto.jpg";
            var disponible = true;
            var orden = 1;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Producto(id, categoriaId, nombreInvalido!, descripcion, precio, urlImagen, disponible, orden));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void CrearProducto_ConPrecioInvalido_DebeLanzarArgumentOutOfRangeException(decimal precioInvalido)
        {
            // Arrange
            var id = Guid.NewGuid();
            var categoriaId = Guid.NewGuid();
            var nombre = "Espresso";
            var descripcion = "Café solo";
            var urlImagen = "foto.jpg";
            var disponible = true;
            var orden = 1;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Producto(id, categoriaId, nombre, descripcion, precioInvalido, urlImagen, disponible, orden));
        }

        [Fact]
        public void CrearProducto_ConCategoriaIdVacia_DebeLanzarArgumentException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var categoriaIdVacia = Guid.Empty;
            var nombre = "Espresso";
            var descripcion = "Café solo";
            var precio = 120m;
            var urlImagen = "foto.jpg";
            var disponible = true;
            var orden = 1;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Producto(id, categoriaIdVacia, nombre, descripcion, precio, urlImagen, disponible, orden));
        }

        [Fact]
        public void CrearProducto_ConOrdenVisualizacionNegativo_DebeLanzarArgumentOutOfRangeException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var categoriaId = Guid.NewGuid();
            var nombre = "Espresso";
            var descripcion = "Café solo";
            var precio = 120m;
            var urlImagen = "foto.jpg";
            var disponible = true;
            var ordenInvalido = -1;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Producto(id, categoriaId, nombre, descripcion, precio, urlImagen, disponible, ordenInvalido));
        }
    }
}
