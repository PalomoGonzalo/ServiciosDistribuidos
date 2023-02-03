using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManager.DTO;
using UserManager.Types;
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
        /// <summary>
        /// Obtiene usuario por legajo de la tabla T_USUARIO
        /// </summary>
        /// <param name="legajo"></param>
        /// <returns></returns>
        [HttpGet("ObtenerUsuarioPorLegajo/{legajo}")]
        [Authorize]
        public async Task<IActionResult> ObtenerUsuarioPorLegajo(int legajo)
        {
            try
            {
                UsuarioDTO usuario = await _usuario.ObtenerUsuarioPorLegajo(legajo);
                if (usuario == null)
                {
                    return NotFound(new HttpBadResponse("no existe el usuario"));
                }
                return Ok(new HttpResponseOk { data = usuario });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }

        /// <summary>
        /// Obtiene usuario por dni de la tabla T_USUARIO
        /// </summary>
        /// <param name="dni"></param>
        /// <returns></returns>
        [HttpGet("ObtenerUsuarioPorDni/{dni}")]
        [Authorize]
        
        public async Task<IActionResult> ObtenerUsuarioPorDni(int dni)
        {
            try
            {
                UsuarioDTO usuario = await _usuario.ObtenerUsuarioPorDni(dni);
                return Ok(new HttpResponseOk { data = usuario });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }

        /// <summary>
        ///  Obtiene usuario todos los usuarios de la tabla T_USUARIO
        /// </summary>
        /// <returns></returns>
        [HttpGet("ObtenerTodosLosUsuarios/")]
        [Authorize]
        public async Task<IActionResult> ObtenerTodosLosUsuarios()
        {
            try
            {
                IEnumerable<UsuarioDTO> usuario = await _usuario.ObtenerTodosLosUsuarios();
                return Ok(new HttpResponseOk{data= usuario});
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }
        /// <summary>
        /// Se cambia los datos del usuario en la tabla t_usuario por legajo, pasandole todo los parametros aunque no lo vaya a cambiar
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("CambiarDatosUsuario")]
        [Authorize]
        public async Task<IActionResult> CambiarDatosUsuario (CambiarDatosUsuarioDTO user)
        {
            try
            {
                CambiarDatosUsuarioDTO usuarioCambiado = await _usuario.CambiarDatosUsuario(user);
                return Ok(new HttpResponseOk{data = usuarioCambiado, msg = "Se cambio Correctamente el usuaro"});
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }
    }
}