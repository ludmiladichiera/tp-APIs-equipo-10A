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


        public HttpResponseMessage Post([FromBody] ArticuloDto articuloDto)
        {
            int nuevoID;
            ArticuloNegocio negocioArticulo = new ArticuloNegocio();
            Articulo articulo = new Articulo();

            try
            {
                // List<string> errores = new List<string>(); //list para mostrar todos los erroers en el Json en vez de mostrar el primero que salte
                if (string.IsNullOrWhiteSpace(articuloDto.Codigo) ||
                    string.IsNullOrWhiteSpace(articuloDto.Nombre) ||
                    string.IsNullOrWhiteSpace(articuloDto.Descripcion))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Todos los campos son obligatorios.");
                }

                if (!decimal.TryParse(articuloDto.Precio.ToString(), out _) || articuloDto.Precio <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "El precio debe ser un número válido y mayor a cero");

                if (!negocioArticulo.existeMarca(articuloDto.IdMarca))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "La marca no existe");

                if (!negocioArticulo.existeCategoria(articuloDto.IdCategoria))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "La categoría no existe");

                if (negocioArticulo.existeCodigo(articuloDto.Codigo))
                    return Request.CreateResponse(HttpStatusCode.Conflict, "El código del artículo ya existe");

                articulo.Codigo = articuloDto.Codigo;
                articulo.Nombre = articuloDto.Nombre;
                articulo.Descripcion = articuloDto.Descripcion;
                articulo.Marca = new Marca { Id = articuloDto.IdMarca };
                articulo.Categoria = new Categoria { Id = articuloDto.IdCategoria };
                articulo.Precio = articuloDto.Precio;

                nuevoID = negocioArticulo.agregarArticuloYDevolverId(articulo);
                return Request.CreateResponse(HttpStatusCode.Created, $"Artículo agregado correctamente con ID {nuevoID}");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.");
            }
        }

        public HttpResponseMessage Put(int id, [FromBody] ArticuloDto articuloDto)
        {
            ArticuloNegocio negocioArticulo = new ArticuloNegocio();
            Articulo modificar = new Articulo();

            try
            {
                if (!negocioArticulo.existeArticuloPorId(id))
                    return Request.CreateResponse(HttpStatusCode.NotFound, "El artículo con ese ID no existe");

                if (string.IsNullOrEmpty(articuloDto.Codigo) || string.IsNullOrEmpty(articuloDto.Nombre) || string.IsNullOrEmpty(articuloDto.Descripcion))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Los campos no pueden estar vacíos");

                if (!decimal.TryParse(articuloDto.Precio.ToString(), out _) || articuloDto.Precio <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "El precio debe ser un número válido y mayor a cero");

                if (!negocioArticulo.existeMarca(articuloDto.IdMarca))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "La marca no existe");

                if (!negocioArticulo.existeCategoria(articuloDto.IdCategoria))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "La categoría no existe");

                if (negocioArticulo.existeCodigoEnOtroArticulo(articuloDto.Codigo, id))
                    return Request.CreateResponse(HttpStatusCode.Conflict, "El código ya pertenece a otro artículo");

                modificar.Codigo = articuloDto.Codigo;
                modificar.Nombre = articuloDto.Nombre;
                modificar.Descripcion = articuloDto.Descripcion;
                modificar.Marca = new Marca { Id = articuloDto.IdMarca };
                modificar.Categoria = new Categoria { Id = articuloDto.IdCategoria };
                modificar.Precio = articuloDto.Precio;
                modificar.Id = id;

                negocioArticulo.modificarArticulo(modificar);
                return Request.CreateResponse(HttpStatusCode.OK, "Artículo actualizado correctamente.");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.");
            }
        }

        // DELETE: api/Articulo/5
        public HttpResponseMessage Delete(int id)
        {
            ArticuloNegocio negocioArticulo = new ArticuloNegocio();
            try
            {

                if (!negocioArticulo.existeArticuloPorId(id))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No se encontró un artículo con el ID especificado.");
                }

                negocioArticulo.EliminarArticulo(id);
                return Request.CreateResponse(HttpStatusCode.OK, "Artículo eliminado correctamente.");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.");
            }
        }
    }
}
