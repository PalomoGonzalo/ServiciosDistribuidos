using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.Helpers
{
    public interface ICliente
    {
        public string ObtenerIpCliente(HttpContext http);
    }


    public class Cliente : ICliente
    {
        public string ObtenerIpCliente(HttpContext http)
        {
            try
            {
                string userIP = http.Connection.RemoteIpAddress.ToString();
                userIP = userIP.Replace("::ffff:", "");
                return userIP;
            }
            catch (System.Exception)
            {    
                throw new Exception("Error al obtener la ip del cliente");
            }    
        }
    }
}