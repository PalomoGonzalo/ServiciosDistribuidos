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
using Productos.Models;
using Productos.Repositorios;
using Productos.Servicios;
using Productos.Types;

namespace Productos.Controllers
{
    [Route("[controller]")]
    public class ProductoController : Controller
    {   

        private readonly IProductos _productos;
        private readonly ILogService _log;


        public ProductoController(IProductos productos, ILogService log)
        {
            _productos = productos;
            _log = log;
        }

        [HttpGet("ObtenerProductosPorPaginacion/{nroDePagina}")]
        public async Task<IActionResult> ObtenerProductosPorPaginacion(int nroDePagina)
        {
            Logger logger = new Logger()
            {
                Servicio = "ObtenerProductosPorPaginacion",
                Usuario = User.FindFirst("USUARIO").Value,
                IP = "198.128.128.222",
                Proceso = "productos",
                Fecha = DateTime.Now,
            };
            


            try
            {
                IEnumerable<ProductosPaginacioDTO> listaProductos = await _productos.ObtenerTodosLosProductosPorPagina(nroDePagina);

                if(listaProductos is null)
                {
                    return NotFound(new HttpBadResponse("No hay datos en esta pagina"));
                }

                _log.GrabarLog(Serilog.Events.LogEventLevel.Information,logger,null);

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