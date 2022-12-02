using System.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Dapper;
using SistemaOrdenDeCompra.Core.Dtos;
using Newtonsoft.Json;

namespace SistemaOrdenDeCompra.Api.Controllers
{
    [Route("[controller]")]
    public class OrdeneCompraController : Controller
    {
        private readonly IConfiguration _config;

        public OrdeneCompraController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet]
        public async Task<IActionResult> Get ()
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString("Connection"));

            string sql = "select * from Libros where Id = 1";

            OrdenesDTO datos = await db.QueryFirstOrDefaultAsync<OrdenesDTO>(sql).ConfigureAwait(false);

            var datitos = datos;

            var lista =JsonConvert.DeserializeObject<List<Data>>(datos.titulo);


          
            return Ok(lista);


        }
        
    }
}