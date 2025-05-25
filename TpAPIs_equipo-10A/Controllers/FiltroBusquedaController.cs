using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Dominio;
using Negocio;
using Newtonsoft.Json;
using TpAPIs_equipo_10A.Models;

namespace TpAPIs_equipo_10A.Controllers
{
    public class FiltroBusquedaController : ApiController
    {
        // GET: api/FiltroBusqueda
        public HttpResponseMessage Get([FromBody] FiltroBusquedaDto filtroDto)
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();
                List<Articulo> articulosFiltrados = new List<Articulo>();

                // Validaciones de los campos
                if (filtroDto.Campo != "Codigo" && filtroDto.Campo != "Nombre" && filtroDto.Campo != "Descripcion" && filtroDto.Campo != "Marca" && filtroDto.Campo != "Categoria" && filtroDto.Campo != "Precio")
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Campo no válido. Campos válidos: Codigo, Nombre, Descripcion, Marca, Categoria, Precio.")
                    };
                }

                if (filtroDto.Campo != "Precio")
                {
                    if (filtroDto.Criterio != "Comienza con" && filtroDto.Criterio != "Termina con" && filtroDto.Criterio != "Contiene")
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent("Criterio no válido. Criterios válidos: Comienza con, Contiene, Termina con.")
                        };
                    }
                }
                else if (filtroDto.Campo == "Precio")
                {
                    if (filtroDto.Criterio != "Menor" && filtroDto.Criterio != "Mayor" && filtroDto.Criterio != "Igual")
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent("Criterio no válido. Criterios válidos: Menor, Mayor, Igual.")
                        };
                    }
                }

                    articulosFiltrados = negocio.filtrar(filtroDto.Campo, filtroDto.Criterio, filtroDto.Filtro);

                    if (articulosFiltrados == null || articulosFiltrados.Count == 0)
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent("No se encontraron artículos que coincidan con el filtro.")
                        };
                    }

                 return new HttpResponseMessage(HttpStatusCode.OK)
                 {
                     Content = new StringContent(JsonConvert.SerializeObject(articulosFiltrados), Encoding.UTF8, "application/json")
                 };
            }
            
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Error en el filtro de búsqueda: {ex.Message}")
                };
            }
        }

    }
}

