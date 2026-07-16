using System;

namespace UnoCatorceCafe.Web.Dominio
{
    public class Producto
    {
        public Guid Id { get; private set; }
        public Guid CategoriaId { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public decimal Precio { get; private set; }
        public string UrlImagen { get; private set; }
        public bool EstaDisponible { get; private set; }
        public int OrdenVisualizacion { get; private set; }

        // Propiedad de navegación de EF Core
        public Categoria? Categoria { get; private set; }

        // Constructor para EF Core
        protected Producto()
        {
            Nombre = string.Empty;
            Descripcion = string.Empty;
            UrlImagen = string.Empty;
        }

        public Producto(
            Guid id, 
            Guid categoriaId, 
            string nombre, 
            string descripcion, 
            decimal precio, 
            string urlImagen, 
            bool estaDisponible, 
            int ordenVisualizacion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(nombre));
            }

            if (precio <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(precio), "El precio del producto debe ser mayor a cero UYU.");
            }

            if (categoriaId == Guid.Empty)
            {
                throw new ArgumentException("El producto debe pertenecer a una categoría válida.", nameof(categoriaId));
            }

            if (ordenVisualizacion < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ordenVisualizacion), "El orden de visualización no puede ser negativo.");
            }

            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            CategoriaId = categoriaId;
            Nombre = nombre;
            Descripcion = descripcion ?? string.Empty;
            Precio = precio;
            UrlImagen = urlImagen ?? string.Empty;
            EstaDisponible = estaDisponible;
            OrdenVisualizacion = ordenVisualizacion;
        }
    }
}
