using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dominio;
using Negocio;
using TpAPIs_equipo_10A.Models;

namespace TpAPIs_equipo_10A.Controllers
{
    public class ArticuloController : ApiController
    {
        // GET: api/Articulo
        public IEnumerable<Articulo> Get()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            ImagenNegocio imagenNegocio = new ImagenNegocio();

            var articulos = negocio.listar();

            foreach (var articulo in articulos)
            {
                var imagenes = imagenNegocio.listar(articulo.Id);

                if (imagenes == null || imagenes.Count == 0)
                {
                    imagenes = new List<Imagen>
            {
                new Imagen
                {
                    Id = 0,
                    IdArticulo = articulo.Id,
                    ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/480px-No_image_available.svg.png"
                }
};
                }

                articulo.Imagenes = imagenes;
            }

            return articulos;
        }

        // GET: api/Articulo/5
        public Articulo Get(int id)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            ImagenNegocio imagenNegocio = new ImagenNegocio();

            Articulo articulo = negocio.listar().Find(x => x.Id == id);

            if (articulo != null)
            {
                var imagenes = imagenNegocio.listar(articulo.Id);

                if (imagenes == null || imagenes.Count == 0)
                {
                    imagenes = new List<Imagen>
            {
                new Imagen
                {
                    Id = 0,
                    IdArticulo = articulo.Id,
                    ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/480px-No_image_available.svg.png"
                }
            };
                }

                articulo.Imagenes = imagenes;
            }

            return articulo;
        }


        // POST: api/Articulo
        public void Post([FromBody] ArticuloDto articulo)
        {
        }

        // PUT: api/Articulo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Articulo/5
        public void Delete(int id)
        {
        }
    }
}
