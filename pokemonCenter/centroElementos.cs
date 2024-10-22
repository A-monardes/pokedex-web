using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dominio;

namespace pokemonCenter
{
    public class centroElementos
    {
        public List<Elemento> listarElementos()
        {
            List<Elemento> lista = new List<Elemento>();
            accesoDatos datos = new accesoDatos();

            try
            {
                datos.setearConsulta("select Id, Descripcion from ELEMENTOS");
                datos.ejecutarLector();

                while (datos.Lector.Read())
                {
                    Elemento auxiliar = new Elemento();
                    auxiliar.Id = (int)datos.Lector["Id"];
                    auxiliar.Descripcion = (string)datos.Lector["Descripcion"];

                    lista.Add(auxiliar);
                }
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { datos.cerrarConexion(); }
        }
    }
}
