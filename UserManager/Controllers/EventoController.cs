using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManager.DTO;
using UserManager.Repositorios;
using UserManager.Types;

namespace UserManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : ControllerBase
    {

        private readonly IEventos _eventos;

        public EventoController(IEventos eventos)
        {
            _eventos = eventos;
        }



        [HttpGet("ObtenerEventosPorId/{id}")]
        public async Task<IActionResult> ObtenerEventosPorId(int id)
        {
            try
            {
                IEnumerable<EventoDTO> eventos = await _eventos.ObtenerEventosPorIdEvento(id);
                return Ok(new HttpResponseOk {data = eventos} );
            }
            catch (System.Exception e)
            {
                
                return BadRequest(new HttpBadResponse(e));
            }
            

        }
    }
}