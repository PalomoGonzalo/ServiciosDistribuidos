using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.Helpers
{
    public interface ICliente
    {
        public string ObtenerIpCliente(IHttpContextAccessor http);
        public string ObtenerDateTimeNowFormatoTimeStamp();
    }


    public class Cliente : ICliente
    {
        /// <summary>
        /// Se obtiene la ip del http, sacando las letras
        /// </summary>
        /// <param name="http"></param>
        /// <returns></returns>
        public string ObtenerIpCliente(IHttpContextAccessor http)
        {
            try
            {
                string userIP = http.HttpContext.Connection.RemoteIpAddress.ToString();
                userIP = userIP.Replace("::ffff:", "");
                return userIP;
            }
            catch (System.Exception)
            {
                throw new Exception("Error al obtener la ip del cliente");
            }
        }
        /// <summary>
        /// Se obtiene datetime now en formato timestamp para insertar en la base de datos
        /// </summary>
        /// <returns></returns>
        public string ObtenerDateTimeNowFormatoTimeStamp()
        {
            try
            {
                DateTime test2 = DateTime.Now;
                var fechaParseada = DateTime.ParseExact(test2.ToString(@"yyyy-MM-dd HH:mm:ss"), @"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                return fechaParseada.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (System.Exception)
            {
                throw new Exception("Error en formatear fecha y hora");
            }
        }
    }
}