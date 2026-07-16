using System;

namespace UnoCatorceCafe.Web.Dominio
{
    public class Categoria
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }
        public int OrdenVisualizacion { get; private set; }
        public bool EstaActiva { get; private set; }

        // Constructor para EF Core
        protected Categoria()
        {
            Nombre = string.Empty;
        }

        public Categoria(Guid id, string nombre, int ordenVisualizacion, bool estaActiva)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(nombre));
            }

            if (ordenVisualizacion < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ordenVisualizacion), "El orden de visualización no puede ser negativo.");
            }

            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Nombre = nombre;
            OrdenVisualizacion = ordenVisualizacion;
            EstaActiva = estaActiva;
        }
    }
}
