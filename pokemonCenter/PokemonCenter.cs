using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;
using System.Collections;

namespace pokemonCenter
{
    public class PokemonCenter
    {
        public List<Pokemon> Listar()
        {
            List<Pokemon> list = new List<Pokemon>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;


            try 
            { 
            conexion.ConnectionString = "server=.\\SQLEXPRESS; database=POKEDEX_DB; integrated security=true";
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = "select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion as Tipo, D.Descripcion as Debilidad, P.IdTipo, P.IdDebilidad, P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D where P.IdTipo = E.Id and D.Id = P.IdDebilidad and P.Activo = 1";
            comando.Connection = conexion;

            conexion.Open();
            lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    Pokemon aux = new Pokemon();

                    aux.Id = (int)lector["Id"];
                    aux.Numero = lector.GetInt32(0);
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];

                    //validación de tipo NULL
                    if (!(lector["UrlImagen"] is DBNull))
                    {
                    aux.UrlImagen = (string)lector["UrlImagen"];
                    }
                    
                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)lector["IdDebilidad"];  
                    aux.Debilidad.Descripcion = (string)lector["Debilidad"];

                    list.Add(aux);
                }
                conexion.Close();

                return list; 
            } 
            catch (Exception ex) 
            { 
                throw ex;
            }
        }
        public void agregar (Pokemon agrega) 
        {
            accesoDatos datos = new accesoDatos();
            try
            {
                //No te olvidees de mapear y agregar todo cuando se agregan nuevas categorias.
                datos.setearConsulta("insert into POKEMONS (Numero, Nombre, Descripcion, Activo, IdTipo, IdDebilidad, UrlImagen)values('" + agrega.Numero +"', '" + agrega.Nombre + "', '" + agrega.Descripcion + "', 1, @IdTipo, @IdDebilidad, @urlImagen)");

                datos.setParametros("@idTipo", agrega.Tipo.Id);
                datos.setParametros("@idDebilidad", agrega.Debilidad.Id);
                datos.setParametros("@urlImagen", agrega.UrlImagen);

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
        public void modificar (Pokemon modificar) 
        {
            accesoDatos datos = new accesoDatos ();
            try
            {
                datos.setearConsulta("update POKEMONS set Numero = @numero, Nombre = @nombre, Descripcion = @descripcion, UrlImagen = @img, IdTipo = @IdTipo, IdDebilidad = @IdDebilidad, Where Id = @Id");
                datos.setParametros("@numero", modificar.Numero);
                datos.setParametros("@nombre", modificar.Nombre);
                datos.setParametros("@descripcion", modificar.Descripcion);
                datos.setParametros("@img", modificar.UrlImagen);
                datos.setParametros("@IdTipo", modificar.Tipo.Id);
                datos.setParametros("@IdDebilidad", modificar.Debilidad.Id);
                datos.setParametros("@Id", modificar.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void eliminar(int id)
        {

            try
            {
            accesoDatos datos = new accesoDatos();
                datos.setearConsulta("delete from POKEMONS where id = @id");
                datos.setParametros ("@id", id);
                datos.ejecutarAccion ();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ocultar (int id)
        {
            try
            {
                accesoDatos datos = new accesoDatos();
                datos.setearConsulta("update POKEMONS set Activo = 0 where id = @id");
                datos.setParametros("@id", id);
                datos.ejecutarAccion ();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Pokemon> filtrar(string campo, string criterio, string filtro)
        {
            List<Pokemon> lista = new List<Pokemon>();
            accesoDatos accesoDatos = new accesoDatos();

            try
            {
                string consulta = "select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion as Tipo, D.Descripcion as Debilidad, P.IdTipo, P.IdDebilidad, P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D where P.IdTipo = E.Id and D.Id = P.IdDebilidad and P.Activo = 1 and ";

                if (campo == "Número")
                {
                    if (criterio == "Mayor a")
                    {
                        consulta += "Numero > " + filtro;
                    }
                    else if (criterio == "Menor a")
                    {
                        consulta += "Numero < " + filtro;
                    }
                    else if (criterio == "Igual a")
                    {
                        consulta += "Numero = " + filtro;
                    }
                }
                else if (campo == "Nombre")
                {
                    if (criterio == "Comienza con")
                    {
                        consulta += "Nombre like '" + filtro + "%' ";
                    }
                    else if (criterio == "Termina con")
                    {
                        consulta += "Nombre like '%" + filtro + "'";
                    }
                    else if (criterio == "Contiene")
                    {
                        consulta += "Nombre like '%" + filtro + "%'";
                    }
                }
                else if (campo == "Descripción")
                {
                    if (criterio == "Comienza con")
                    {
                        consulta += "P.Descripcion like '" + filtro + "%' ";
                    }
                    else if (criterio == "Termina con")
                    {
                        consulta += "P.Descripcion like '%" + filtro + "'";
                    }
                    else if (criterio == "Contiene")
                    {
                        consulta += "P.Descripcion like '%" + filtro + "%'";
                    }
                }

                accesoDatos.setearConsulta(consulta);
                accesoDatos.ejecutarLector();

                while (accesoDatos.Lector.Read())
                {
                    Pokemon aux = new Pokemon();

                    aux.Id = (int)accesoDatos.Lector["Id"];
                    aux.Numero = accesoDatos.Lector.GetInt32(0);
                    aux.Nombre = (string)accesoDatos.Lector["Nombre"];
                    aux.Descripcion = (string)accesoDatos.Lector["Descripcion"];

                    //validación de tipo NULL
                    if (!(accesoDatos.Lector["UrlImagen"] is DBNull))
                    {
                        aux.UrlImagen = (string)accesoDatos.Lector["UrlImagen"];
                    }

                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)accesoDatos.Lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)accesoDatos.Lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)accesoDatos.Lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)accesoDatos.Lector["Debilidad"];

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
