using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dominio;

namespace Negocio
{
    internal class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=PROMOS_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = (@"SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.IdMarca, M.Descripcion AS Marca, 
                     A.IdCategoria, C.Descripcion AS Categoria, A.Precio, 
                     MIN(I.ImagenUrl) AS ImagenesUrl
                     FROM ARTICULOS A
                     JOIN MARCAS M ON A.IdMarca = M.Id
                     LEFT JOIN CATEGORIAS C ON A.IdCategoria = C.Id
                     LEFT JOIN IMAGENES I ON A.Id = I.IdArticulo
                     GROUP BY A.Id, A.Codigo, A.Nombre, A.Descripcion, A.IdMarca, M.Descripcion, A.IdCategoria, C.Descripcion, A.Precio");
               
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read()) //posiciona el puntero
                {
                    Articulo aux = new Articulo();

                    aux.Id = (int)lector["Id"];

                    aux.Codigo = lector["Codigo"] != DBNull.Value ? (string)lector["Codigo"] : "No especifica";
                    aux.Nombre = lector["Nombre"] != DBNull.Value ? (string)lector["Nombre"] : "No especifica";
                    aux.Descripcion = lector["Descripcion"] != DBNull.Value ? (string)lector["Descripcion"] : "No especifica";
                   
                    aux.Precio = lector["Precio"] != DBNull.Value ? (decimal)lector["Precio"] : 0;


                    aux.Marca = new Marca();
                    
                    aux.Marca.Descripcion = lector["Marca"] != DBNull.Value ? lector["Marca"].ToString() : "NO especifica";

                    aux.Categoria = new Categoria();
                    aux.Categoria.Descripcion = lector["Categoria"] != DBNull.Value ? lector["Categoria"].ToString() : "NO especifica";



                   
                    if (aux.Imagenes == null)
                        aux.Imagenes = new List<Imagen>();

                    if (!(lector["ImagenesUrl"] is DBNull))
                    {
                        aux.Imagenes.Add(new Imagen(lector["ImagenesUrl"].ToString()));
                    }
                    else
                    {
                        aux.Imagenes.Add(new Imagen("https://media.istockphoto.com/id/1128826884/es/vector/ning%C3%BAn-s%C3%ADmbolo-de-vector-de-imagen-falta-icono-disponible-no-hay-galer%C3%ADa-para-este-momento.jpg?s=612x612&w=0&k=20&c=9vnjI4XI3XQC0VHfuDePO7vNJE7WDM8uzQmZJ1SnQgk="));
                    }


                    lista.Add(aux);
                }

                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int agregarArticuloYDevolverId(Articulo nuevoArticulo)
        {
            int idGenerado;
            SqlConnection conexion = new SqlConnection("server=.\\SQLEXPRESS; database=CATALOGO_P3_DB; integrated security=true");
            SqlCommand comando = new SqlCommand();

            try
            {
                comando.Connection = conexion;
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = @"INSERT INTO Articulos (Codigo, Nombre, Descripcion, Precio, IdMarca, IdCategoria) 
                        VALUES (@Codigo, @Nombre, @Descripcion, @Precio, @IdMarca, @IdCategoria);
                        SELECT SCOPE_IDENTITY();";

                comando.Parameters.AddWithValue("@Codigo", string.IsNullOrWhiteSpace(nuevoArticulo.Codigo)
                    ? (object)DBNull.Value
                    : nuevoArticulo.Codigo);

                comando.Parameters.AddWithValue("@Nombre", string.IsNullOrWhiteSpace(nuevoArticulo.Nombre)
                    ? (object)DBNull.Value
                    : nuevoArticulo.Nombre);

                comando.Parameters.AddWithValue("@Descripcion", string.IsNullOrWhiteSpace(nuevoArticulo.Descripcion)
                    ? (object)DBNull.Value
                    : nuevoArticulo.Descripcion);

                comando.Parameters.AddWithValue("@Precio", nuevoArticulo.Precio.HasValue
                    ? (object)nuevoArticulo.Precio.Value
                    : DBNull.Value);

                comando.Parameters.AddWithValue("@IdMarca", nuevoArticulo.Marca?.Id ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@IdCategoria", nuevoArticulo.Categoria?.Id ?? (object)DBNull.Value);

                conexion.Open();
                idGenerado = Convert.ToInt32(comando.ExecuteScalar());
                return idGenerado;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }
        public void agregarImagenUrl(int idArticulo, List<Imagen> imagenes)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                foreach (Imagen imagen in imagenes)
                {
                    datos.setearConsulta("INSERT INTO IMAGENES (IdArticulo, ImagenUrl) " +
                                         "VALUES (@IdArticulo, @ImagenUrl)");
                    datos.setearParametro("@IdArticulo", idArticulo);
                    datos.setearParametro("@ImagenUrl", imagen.ImagenUrl);
                    datos.ejecutarAccion();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public Articulo ObtenerArticuloId(int idArticulo)
        {
            Articulo articulo = null;
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT A.Id, A.Nombre, A.Descripcion, A.Precio, A.IdMarca, A.IdCategoria, M.Descripcion AS Marca, C.Descripcion AS Categoria " +
                                     "FROM ARTICULOS A " +
                                     "INNER JOIN MARCAS M ON A.IdMarca = M.Id " +
                                     "INNER JOIN CATEGORIAS C ON A.IdCategoria = C.Id " +
                                     "WHERE A.Id= @Id");
                datos.setearParametro("@Id", idArticulo);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    Articulo art = new Articulo();
                    art.Id = (int)datos.Lector["Id"];
                    art.Nombre = datos.Lector["Nombre"].ToString();
                    art.Descripcion = datos.Lector["Descripcion"].ToString();
                    art.Marca = new Marca((int)datos.Lector["IdMArca"], datos.Lector["Marca"].ToString());
                    art.Categoria = new Categoria((int)datos.Lector["IdCategoria"], datos.Lector["Categoria"].ToString());
                    art.Precio = (decimal)datos.Lector["Precio"];
                    articulo = art;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
            return articulo;
        }
        public void Modificar(Articulo modificar)
        {
            AccesoDatos datos = new AccesoDatos();
            modificarArticulo(modificar);
            int idArticulo = datos.obtenerIdArt(modificar.Codigo);
            modificarImagenUrl(idArticulo, modificar.Imagenes);
        }
        public void modificarArticulo(Articulo modificar)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE ARTICULOS set Codigo = @Codigo, Nombre = @Nombre, Descripcion = @Descripcion, IdMarca = @IdMarca," +
                                     "IdCategoria = @IdCategoria, Precio = @Precio where Id = @Id");
                datos.setearParametro("@Codigo", modificar.Codigo);
                datos.setearParametro("@Nombre", modificar.Nombre);
                datos.setearParametro("@Descripcion", modificar.Descripcion);
                datos.setearParametro("@IdCategoria", modificar.Categoria.Id);
                datos.setearParametro("@IdMarca", modificar.Marca.Id);
                datos.setearParametro("@Precio", modificar.Precio);
                datos.setearParametro("@Id", modificar.Id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void modificarImagenUrl(int idArticulo, List<Imagen> imagenes)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                foreach (Imagen imagen in imagenes)
                {
                    datos.setearConsulta("INSERT INTO IMAGENES (IdArticulo, ImagenUrl) " +
                                         "VALUES (@IdArticulo, @ImagenUrl)");
                    datos.setearParametro("@IdArticulo", idArticulo);
                    datos.setearParametro("@ImagenUrl", imagen.ImagenUrl);
                    datos.ejecutarAccion();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
       
        public void EliminarArticulo(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from ARTICULOS where id= @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = @"SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio,
                            M.Descripcion AS Marca, C.Descripcion AS Categoria,
                            MIN(I.ImagenUrl) AS ImagenesUrl
                            FROM ARTICULOS A
                            JOIN MARCAS M ON A.IdMarca = M.Id
                            LEFT JOIN CATEGORIAS C ON A.IdCategoria = C.Id
                            LEFT JOIN IMAGENES I ON A.Id = I.IdArticulo
                            WHERE ";


                if (campo == "Codigo")
                {
                    consulta += "A.Codigo LIKE @filtro";
                }
                else if (campo == "Nombre")
                {
                    consulta += "A.Nombre LIKE @filtro";
                }
                else if (campo == "Descripcion")
                {
                    consulta += "A.Descripcion LIKE @filtro";
                }
                else if (campo == "Marca")
                {
                    consulta += "M.Descripcion LIKE @filtro";
                }
                else if (campo == "Categoria")
                {
                    consulta += "C.Descripcion LIKE @filtro";
                }
                else if (campo == "Precio")
                {

                    if (decimal.TryParse(filtro, out decimal precio))
                    {
                        switch (criterio)
                        {
                            case "Mayor a":
                                consulta += "A.Precio > @filtro";
                                break;
                            case "Menor a":
                                consulta += "A.Precio < @filtro";
                                break;
                            default:
                                consulta += "A.Precio = @filtro";
                                break;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("El filtro para el precio debe ser un número válido.");
                    }
                }


                consulta += " GROUP BY A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio, M.Descripcion, C.Descripcion";

                datos.setearConsulta(consulta);


                if (campo == "Precio")
                {

                    datos.setearParametro("@filtro", filtro);
                }
                else
                {

                    datos.setearParametro("@filtro", "%" + filtro + "%");
                }


                datos.ejecutarLectura();


                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = datos.Lector["Codigo"].ToString();
                    aux.Nombre = datos.Lector["Nombre"].ToString();
                    aux.Descripcion = datos.Lector["Descripcion"].ToString();
                    aux.Precio = (decimal)datos.Lector["Precio"];

                    aux.Marca = new Marca { Descripcion = datos.Lector["Marca"].ToString() };
                    aux.Categoria = new Categoria { Descripcion = datos.Lector["Categoria"].ToString() };

                    aux.Imagenes = new List<Imagen>();
                    if (!(datos.Lector["ImagenesUrl"] is DBNull))
                        aux.Imagenes.Add(new Imagen(datos.Lector["ImagenesUrl"].ToString()));

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
