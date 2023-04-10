using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Productos.DTOS;

namespace Productos.Controllers
{
    [Route("[controller]")]
    public class ProductoController : Controller
    {   

        private readonly IConfiguration _config;

        public ProductoController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("ObtenerProductos")]
        public async Task<IActionResult> ObtenerCategoria()
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            
            string sql  = $@"SELECT * FROM Productos";

            IEnumerable<ProductoDTO> listarCategoria = await db.QueryAsync<ProductoDTO>(sql).ConfigureAwait(false);
            
            return Ok(listarCategoria);

        }
 
    }
}