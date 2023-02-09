using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserManager.DTO
{
    public class EventoDTO
    {
        public int Id_Evento { get; set; }
        public string EventoNombre { get; set; }
        public string Usuario { get; set; }
       
        [JsonIgnore]
        public string Dato { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public dynamic DatoUsuario { get; set;}
    }
}