using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaOrdenDeCompra.Core.Dtos
{
    public class Data
    {

        [JsonProperty("indice")]
        public string indice { get; set; }

        [JsonProperty("items")]
        public string items { get; set; }

   
        
    }
}
