using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UnoCatorceCafe.Web.Dominio;

namespace UnoCatorceCafe.Web.Datos
{
    public static class DbInicializador
    {
        public static async Task Inicializar(CafeteriaDbContext contexto)
        {
            // Crear la base de datos si no existe
            await contexto.Database.EnsureCreatedAsync();

            // Buscar si ya existen categorías (ya fue seedead)
            if (await contexto.Categorias.AnyAsync())
            {
                return;   // DB ya inicializada
            }

            // --- Categorías ---
            var catCafe = new Categoria(Guid.NewGuid(), "☕ Café de Especialidad", 1, true);
            var catBebidasFrias = new Categoria(Guid.NewGuid(), "🧊 Bebidas Frías", 2, true);
            var catPasteleria = new Categoria(Guid.NewGuid(), "🥐 Pastelería Artesanal", 3, true);

            await contexto.Categorias.AddRangeAsync(catCafe, catBebidasFrias, catPasteleria);
            await contexto.SaveChangesAsync();

            // --- Productos ---
            var productos = new[]
            {
                // Café de Especialidad
                new Producto(Guid.NewGuid(), catCafe.Id, "Espresso", "Extracción individual concentrada de granos seleccionados.", 110m, "", true, 1),
                new Producto(Guid.NewGuid(), catCafe.Id, "Flat White", "Doble shot de espresso con una capa fina de leche emulsionada sedosa.", 160m, "", true, 2),
                new Producto(Guid.NewGuid(), catCafe.Id, "Capuccino", "Doble espresso con partes iguales de leche vaporizada y crema de leche.", 150m, "", true, 3),
                new Producto(Guid.NewGuid(), catCafe.Id, "Filtrado V60", "Café de origen filtrado por goteo, resaltando notas florales y frutales.", 170m, "", true, 4),

                // Bebidas Frías
                new Producto(Guid.NewGuid(), catBebidasFrias.Id, "Iced Latte", "Doble espresso vertido sobre leche fría y cubos de hielo.", 160m, "", true, 1),
                new Producto(Guid.NewGuid(), catBebidasFrias.Id, "Cold Brew", "Café de especialidad extraído en frío por 18 horas, dulce y refrescante.", 180m, "", true, 2),
                new Producto(Guid.NewGuid(), catBebidasFrias.Id, "Espresso Tonic", "Agua tónica premium con un doble shot de espresso frío y rodaja de lima.", 190m, "", true, 3),

                // Pastelería
                new Producto(Guid.NewGuid(), catPasteleria.Id, "Croissant de Manteca", "Hojaldre clásico francés súper aireado y crujiente, pintado con almíbar.", 140m, "", true, 1),
                new Producto(Guid.NewGuid(), catPasteleria.Id, "Cookie con Chips de Chocolate", "Galleta húmeda por dentro repleta de chips de chocolate belga semi-amargo.", 120m, "", true, 2),
                new Producto(Guid.NewGuid(), catPasteleria.Id, "Tostón con Palta", "Pan de masa madre tostado, palta fresca, oliva y semillas de sésamo.", 220m, "", true, 3)
            };

            await contexto.Productos.AddRangeAsync(productos);
            await contexto.SaveChangesAsync();
        }
    }
}
