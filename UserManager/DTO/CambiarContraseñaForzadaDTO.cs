using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.DTO
{
    public class CambiarContraseñaForzadaDTO
    {
        public int Legajo { get; set; }
        public string PasswordNueva { get; set; }
    }
}