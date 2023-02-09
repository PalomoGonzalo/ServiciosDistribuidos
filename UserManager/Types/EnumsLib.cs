using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.Types
{
    public class EnumsLib
    {
        public enum TipoEstado
        {
            Activo = 1,
            Baja = 0,
        }

        public enum EventosEstados
        {
            EventoReprogramado = 1,
            EventoBajaLogica = 2, 
            EventoCambiarContraseñaForzada = 3,

        }


    }
}