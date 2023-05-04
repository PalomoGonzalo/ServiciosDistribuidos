using System.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Productos.DTOS;
using Productos.Models;
using Productos.Repositorios;
using Productos.Servicios;
using Productos.Types;
using Microsoft.Extensions.Configuration;
using Productos.Helpers;

namespace Productos.Controllers
{
    [Route("[controller]")]
    public class ProductoController : Controller
    {

        private readonly IProductos _productos;
        private readonly ILogService _log;
        private readonly IConfiguration _config;
        private readonly ICLiente _cliente;

        public ProductoController(IProductos productos, ILogService log, IConfiguration config, ICLiente cliente)
        {
            _productos = productos;
            _log = log;
            _config = config;
            _cliente = cliente;
        }

        [HttpGet("ObtenerProductosPorPaginacion/{nroDePagina}")]
        public async Task<IActionResult> ObtenerProductosPorPaginacion(int nroDePagina)
        {
            /*
                    Logger logger = new Logger()
            {
                Servicio = "ObtenerProductosPorPaginacion",
                Usuario = User.FindFirst("USUARIO").Value,
                IP = "198.128.128.222",
                Proceso = "productos",
                Fecha = DateTime.Now,
            };

            */
            try
            {
                Logger logger = _cliente.ObtenerClaimsLogger(this.HttpContext,"ObtenerProductosPorPaginacion","Productos");
                IEnumerable<ProductosPaginacioDTO> listaProductos = await _productos.ObtenerTodosLosProductosPorPagina(nroDePagina);

                if (listaProductos is null)
                {
                    return NotFound(new HttpBadResponse("No hay datos en esta pagina"));
                }

                _log.GrabarLog(Serilog.Events.LogEventLevel.Information, logger, null);

                return Ok(new HttpResponseOk { data = listaProductos });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }


        [HttpGet("TestItemClaims")]
        public IActionResult TestItemClaims()
        {
            // si estan en la misma red utilizar este metodo
            /* string myIp = string.Empty;

             foreach (System.Net.IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
             {
                 if (ip.AddressFamily == AddressFamily.InterNetwork)
                 {
                     myIp = ip.ToString();
                     break;
                 }
             }
             return Ok(myIp);*/

            // se obtiene la ip publica
            try
            {
                string userIP = HttpContext.Connection.RemoteIpAddress.ToString();
                userIP = userIP.Replace("::ffff:", "");
                return Ok(userIP);
            }
            catch (System.Exception)
            {
                throw new Exception("Error al obtener la ip del cliente");
            }
        }

        [HttpGet("DescargarLog/{archivo}")]
        public async Task<IActionResult> DescargarLog(string archivo)
        {

         String path = "";
        
        if (archivo == null)
            return Content($"no existe el fichero {archivo}");

        path = _config.GetSection("LoggingConf").GetValue<string>("outputPath").Trim() + archivo.ToString();

        if (!System.IO.File.Exists(path))
        {
            return BadRequest($"No se existe el documento {archivo}");
        }

        MemoryStream memory = new MemoryStream();
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;

        return File(memory, "text/plain", Path.GetFileName(path));;
        }


        [HttpGet("ObtenerProductoPorId/{id}")]
        public async Task<IActionResult> ObtenerProductoPorId(int id)
        {
            try
            {
                ProductoDTO producto = await _productos.ObtenerProductoPorId(id);
                return Ok(new HttpResponseOk { data = producto });
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

                return Ok(new HttpResponseOk { data = listaProductos });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }

        [HttpPost("DarDeBajaProductoLogico")]
        public async Task<IActionResult> DarDeBajaProductoLogico([FromBody] ProductoEliminarIdDTO idProducto)
        {
            try
            {
                ProductoEliminarDTO productoEliminado = await _productos.DarDeBajaProductoLogico(idProducto.Id_producto);
                return Ok(new HttpResponseOk { data = productoEliminado, msg = "Se dio de baja correctamente" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }
    }
}