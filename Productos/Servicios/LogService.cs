using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Productos.Models;
using Serilog;
using Serilog.Events;

namespace Productos.Servicios
{
    public interface ILogService
    {
        void GrabarLog(LogEventLevel Level,Logger Mensaje,Exception Exception);
    }

    public class LoggService : ILogService
    {
        private static Int32 IdLog=1;

        public LoggService()
        {

        }

        public void GrabarLog(LogEventLevel Level,Logger Logger,Exception Exception){

            
            
            Logger.IdServicio= IdLog;
            Logger.Duracion = DateTime.Now - Logger.Fecha;

            if(Level==LogEventLevel.Error){
                Logger.Estado = "nok";
                Log.Write(Level,JsonConvert.SerializeObject(Logger) +" || "+ Exception);
                IdLog++;
            }
            else{
                Logger.Estado = "ok";
                Log.Write(Level,JsonConvert.SerializeObject(Logger));
                IdLog++;
            }
        }
    }
}