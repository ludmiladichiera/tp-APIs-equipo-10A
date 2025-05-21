using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpAPIs_equipo_10A.Models
{
    public class ArticuloDto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdMarca { get; set; }
        public int IdCategoria { get; set; }
        public decimal? Precio { get; set; }
    }
}