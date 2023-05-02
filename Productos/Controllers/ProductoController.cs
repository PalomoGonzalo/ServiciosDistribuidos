using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Productos.DTOS;
using Productos.Repositorios;
using Productos.Types;

namespace Productos.Controllers
{
    [Route("[controller]")]
    public class ProductoController : Controller
    {   

        private readonly IProductos _productos;
        private readonly ILogger<ProductoController> _logger;

        public ProductoController(IProductos productos, ILogger<ProductoController> logger)
        {
            _productos = productos;
            _logger = logger;
        }

        [HttpGet("ObtenerProductosPorPaginacion/{nroDePagina}")]
        public async Task<IActionResult> ObtenerProductosPorPaginacion(int nroDePagina)
        {
            try
            {
                IEnumerable<ProductosPaginacioDTO> listaProductos = await _productos.ObtenerTodosLosProductosPorPagina(nroDePagina);

                if(listaProductos is null)
                {
                    return NotFound(new HttpBadResponse("No hay datos en esta pagina"));
                }

                return Ok(new HttpResponseOk{data = listaProductos});   
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }


        [HttpGet("TestItemClaims")]
        public IActionResult TestItemClaims()
        {
            try
            {
                string test = _productos.TestClaims(this.HttpContext);
                _logger.LogInformation("Haciendo consulta claims");
                return Ok(new HttpResponseOk{data = test});
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }

        [HttpGet("ObtenerProductoPorId/{id}")]
        public async Task<IActionResult> ObtenerProductoPorId(int id)
        {
            try
            {
                _logger.LogInformation("Haciendo consulta ObtenerProductoPorId {@id} " ,id);
                ProductoDTO producto = await _productos.ObtenerProductoPorId(id);



                return Ok(new HttpResponseOk{data = producto});
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }

        }
        [HttpGet("ObtenerProductosPorNombre/{nombre}")]
        public async Task<IActionResult> ObtenerProductosPorNombre(string nombre)
        {
            try
            {
                IEnumerable<ProductoDTO> listaProductos = await _productos.ObtenerProductosPorNombre(nombre);

                return Ok(new HttpResponseOk{data = listaProductos});
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }

        [HttpPost("DarDeBajaProductoLogico")]
        public async Task<IActionResult> DarDeBajaProductoLogico([FromBody]ProductoEliminarIdDTO idProducto)
        {
            try
            {
                ProductoEliminarDTO productoEliminado = await _productos.DarDeBajaProductoLogico(idProducto.Id_producto);
                return Ok(new HttpResponseOk{data = productoEliminado,msg = "Se dio de baja correctamente"});
            }
            catch (System.Exception ex)
            {
                
                return BadRequest(new HttpBadResponse(ex));
            }
        }
    }
}