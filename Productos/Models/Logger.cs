using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productos.Models
{
    public class Logger
    {
        public Int32 IdServicio { get; set; }
        public String Servicio { get; set; }
        public String Usuario { get; set; }
        public String IP { get; set; }
        public String Proceso { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Duracion { get; set; }
        public String Estado { get; set; }
    }
}