using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManager.DTO;
using UserManager.Repositorios;

namespace UserManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly ILogin _login;

        public UsuarioController(ILogin login)
        {
            _login = login;
        }


        [Authorize]
        [HttpGet("ObtenerUsuarioPorLegajo/{legajo}")]
        public async Task<IActionResult> ObtenerUsuarioPorLegajo(int legajo)
        {
            UsuarioDTO listUsuario = await _login.ObtenerUsuarioPorLegajo(legajo);
            if (listUsuario == null)
            {
                return NotFound("no existe el usuario");
            }
            return Ok(listUsuario);
        }



    }
}