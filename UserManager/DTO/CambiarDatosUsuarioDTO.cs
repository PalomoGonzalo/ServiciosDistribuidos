using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.DTO
{
    public class CambiarDatosUsuarioDTO
    {
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public int Dni { get; set; }
        
    }
}