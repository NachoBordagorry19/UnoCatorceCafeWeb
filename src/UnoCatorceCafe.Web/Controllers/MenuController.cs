using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnoCatorceCafe.Web.Datos;
using UnoCatorceCafe.Web.Modelos;

namespace UnoCatorceCafe.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly CafeteriaDbContext _contexto;

        public MenuController(CafeteriaDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaMenuDto>>> ObtenerMenu()
        {
            // Obtener todas las categorías activas y ordenadas
            var categorias = await _contexto.Categorias
                .Where(c => c.EstaActiva)
                .OrderBy(c => c.OrdenVisualizacion)
                .ToListAsync();

            // Obtener todos los productos disponibles y ordenados
            var productos = await _contexto.Productos
                .Where(p => p.EstaDisponible)
                .OrderBy(p => p.OrdenVisualizacion)
                .ToListAsync();

            var resultado = new List<CategoriaMenuDto>();

            // Agrupar en los DTOs de salida
            foreach (var cat in categorias)
            {
                var productosDeCat = productos
                    .Where(p => p.CategoriaId == cat.Id)
                    .Select(p => new ProductoMenuDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        Precio = p.Precio,
                        UrlImagen = p.UrlImagen
                    })
                    .ToList();

                resultado.Add(new CategoriaMenuDto
                {
                    Id = cat.Id,
                    Nombre = cat.Nombre,
                    OrdenVisualizacion = cat.OrdenVisualizacion,
                    Productos = productosDeCat
                });
            }

            return Ok(resultado);
        }
    }
}
