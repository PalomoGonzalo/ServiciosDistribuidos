using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.DTO
{
    public class EventoDTO
    {
        public int Id_Evento { get; set; }
        public int EventoNombre { get; set; }
        public string Usuario { get; set; }
        public string Dato { get; set; }
        public DateTime Fecha_Creacion { get; set; }
    }
}