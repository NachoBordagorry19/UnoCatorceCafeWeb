using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using UnoCatorceCafe.Web.Datos;
using UnoCatorceCafe.Web.Dominio;

namespace UnoCatorceCafe.Tests.Controladores
{
    public class MenuControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public MenuControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ObtenerMenu_DebeRetornarCategoriasYProductosOrdenadosYFiltrados()
        {
            // Limpiar y seedear base de datos de test
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CafeteriaDbContext>();
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();

                // Seedear Categorias
                var catCafe = new Categoria(Guid.NewGuid(), "Café", 1, true);
                var catTe = new Categoria(Guid.NewGuid(), "Té", 2, true);
                var catInactiva = new Categoria(Guid.NewGuid(), "Inactiva", 3, false);

                await db.Categorias.AddRangeAsync(catCafe, catTe, catInactiva);

                // Seedear Productos
                var prodFlat = new Producto(Guid.NewGuid(), catCafe.Id, "Flat White", "Café doble", 160m, "flat.jpg", true, 2);
                var prodEspresso = new Producto(Guid.NewGuid(), catCafe.Id, "Espresso", "Café solo", 100m, "espresso.jpg", true, 1);
                var prodNoDisponible = new Producto(Guid.NewGuid(), catCafe.Id, "Irish Coffee", "Café con whisky", 250m, "irish.jpg", false, 3);
                var prodTeNegro = new Producto(Guid.NewGuid(), catTe.Id, "Té Negro", "Hojas enteras", 120m, "te.jpg", true, 1);

                await db.Productos.AddRangeAsync(prodFlat, prodEspresso, prodNoDisponible, prodTeNegro);
                await db.SaveChangesAsync();
            }

            // Act
            var respuesta = await _client.GetAsync("/api/menu");

            // Assert
            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);

            var menu = await respuesta.Content.ReadFromJsonAsync<List<CategoriaMenuDto>>();
            Assert.NotNull(menu);
            
            // Deben venir solo las 2 categorías activas
            Assert.Equal(2, menu.Count);

            // Primera categoría debe ser Café (OrdenVisualizacion = 1)
            var cat1 = menu[0];
            Assert.Equal("Café", cat1.Nombre);
            
            // Café debe tener 2 productos (excluyendo el no disponible) ordenados por OrdenVisualizacion (1: Espresso, 2: Flat White)
            Assert.Equal(2, cat1.Productos.Count);
            Assert.Equal("Espresso", cat1.Productos[0].Nombre);
            Assert.Equal("Flat White", cat1.Productos[1].Nombre);

            // Segunda categoría debe ser Té (OrdenVisualizacion = 2)
            var cat2 = menu[1];
            Assert.Equal("Té", cat2.Nombre);
            Assert.Single(cat2.Productos);
            Assert.Equal("Té Negro", cat2.Productos[0].Nombre);
        }
    }

    // DTOs auxiliares para deserialización en pruebas
    public class CategoriaMenuDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int OrdenVisualizacion { get; set; }
        public List<ProductoMenuDto> Productos { get; set; } = new();
    }

    public class ProductoMenuDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string UrlImagen { get; set; } = string.Empty;
    }
}
