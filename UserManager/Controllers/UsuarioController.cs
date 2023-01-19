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
        private readonly IUsuario _usuario;

        public UsuarioController(IUsuario user)
        {
            _usuario = user;
        }


        
        [HttpGet("ObtenerUsuarioPorLegajo/{legajo}")]
        [Authorize]
        public async Task<IActionResult> ObtenerUsuarioPorLegajo(int legajo)
        {
            UsuarioDTO usuario = await _usuario.ObtenerUsuarioPorLegajo(legajo);
            if (usuario == null)
            {
                return NotFound("no existe el usuario");
            }
            return Ok(usuario);
        }

        
        [HttpGet("ObtenerUsuarioPorDni/{dni}")]
        [Authorize]
        public async Task<IActionResult> ObtenerUsuarioPorDni(int dni)
        {
            UsuarioDTO usuario = await _usuario.ObtenerUsuarioPorDni(dni);
            if (usuario == null)
            {
                return NotFound("no existe el usuario");
            }
            return Ok(usuario);
        }
        
        
        [HttpGet("ObtenerTodosLosUsuarios/")]
        public async Task<IActionResult> ObtenerTodosLosUsuarios()
        {
            IEnumerable<UsuarioDTO> usuario = await _usuario.ObtenerTodosLosUsuarios();
            if (usuario == null)
            {
                return NotFound("ERROR EN TRAER A LOS USUARIOS");
            }
            return Ok(usuario);
        }
    }
}