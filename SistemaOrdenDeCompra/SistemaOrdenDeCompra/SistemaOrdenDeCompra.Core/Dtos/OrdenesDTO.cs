using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaOrdenDeCompra.Core.Dtos
{
    public class OrdenesDTO
    {

        public int id { get; set; }

        [JsonProperty("titulo")]
      
        public string titulo { get; set; }
    }
}
