using System;
using System.Collections.Generic;

namespace UnoCatorceCafe.Web.Modelos
{
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
