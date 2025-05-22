using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpAPIs_equipo_10A.Models
{
    public class ImagenDto
    {
        public int IdArticulo { get; set; }
        public List<string> Imagenes { get; set; }
    }
}