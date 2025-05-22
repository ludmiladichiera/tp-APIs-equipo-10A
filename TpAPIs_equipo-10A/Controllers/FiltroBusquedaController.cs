using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TpAPIs_equipo_10A.Controllers
{
    public class FiltroBusquedaController : ApiController
    {
        // GET: api/FiltroBusqueda
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/FiltroBusqueda/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/FiltroBusqueda
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/FiltroBusqueda/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/FiltroBusqueda/5
        public void Delete(int id)
        {
        }
    }
}
