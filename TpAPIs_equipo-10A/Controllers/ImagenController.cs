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
    public class ImagenController : ApiController
    {
        // GET: api/Imagen
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Imagen/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Imagen
        public HttpResponseMessage Post([FromBody] ImagenDto dto)
        {
            try
            {
                if (dto == null || dto.Imagenes == null || dto.Imagenes.Count == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No se recibieron imágenes para agregar.");

                if (dto.IdArticulo <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Id de artículo inválido.");

                ImagenNegocio negocio = new ImagenNegocio();

                foreach (var url in dto.Imagenes)
                {
                    Imagen imagen = new Imagen
                    {
                        IdArticulo = dto.IdArticulo,
                        ImagenUrl = url
                    };

                    negocio.Agregar(imagen);
                }

                return Request.CreateResponse(HttpStatusCode.OK, "Imágenes agregadas correctamente.");
            }
            catch (Exception)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.");
            }
        }

        // PUT: api/Imagen/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Imagen/5
        public void Delete(int id)
        {
        }
    }
}
