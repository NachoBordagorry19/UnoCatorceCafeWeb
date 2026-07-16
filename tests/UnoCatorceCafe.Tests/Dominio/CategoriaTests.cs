using System;
using Xunit;
using UnoCatorceCafe.Web.Dominio;

namespace UnoCatorceCafe.Tests.Dominio
{
    public class CategoriaTests
    {
        [Fact]
        public void CrearCategoria_ConDatosValidos_DebeCrearInstancia()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nombre = "Café de Especialidad";
            var orden = 1;
            var activa = true;

            // Act
            var categoria = new Categoria(id, nombre, orden, activa);

            // Assert
            Assert.Equal(id, categoria.Id);
            Assert.Equal(nombre, categoria.Nombre);
            Assert.Equal(orden, categoria.OrdenVisualizacion);
            Assert.Equal(activa, categoria.EstaActiva);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CrearCategoria_ConNombreInvalido_DebeLanzarArgumentException(string? nombreInvalido)
        {
            // Arrange
            var id = Guid.NewGuid();
            var orden = 1;
            var activa = true;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Categoria(id, nombreInvalido!, orden, activa));
        }

        [Fact]
        public void CrearCategoria_ConOrdenVisualizacionNegativo_DebeLanzarArgumentOutOfRangeException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nombre = "Café de Especialidad";
            var ordenInvalido = -1;
            var activa = true;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new Categoria(id, nombre, ordenInvalido, activa));
        }
    }
}
