using Microsoft.AspNetCore.Mvc;
using UserManager.DTO;
using UserManager.Repositorios;

namespace UserManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogin _login;

        public LoginController(ILogin login)
        {
            _login = login;
        }

      /*  [HttpGet("obtenerUsuarios")]
        public async Task<IActionResult> OtenerUsuarios()
        {
            IEnumerable<LoginDTO> listUsuario = await _login.Loguear();
            if (listUsuario == null)
            {
                return NotFound("no existe el usuario");
            }
            return Ok(listUsuario);
        }*/

        [HttpPost("Login")]
        public  async Task <IActionResult> Login([FromBody] LoginDTO user)
        {
            if (user == null)
                return BadRequest("Error datos no validos");
            string token = await _login.Loguear(user);
            
            if (token == null)
            {
                return BadRequest("error en el usuario o contrase�a ");
            }
            
            return Ok(token);
        }

        [HttpPost("CrearUsuario")]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDTO user)
        {
            LoginDTO usuarioExiste =  await _login.ObtenerUsuarioLogin(user.Usuario);

            if(usuarioExiste!=null)
            {
                return BadRequest($"El usario {user.Usuario} ya existe");
            }
            CrearUsuarioDTO usuarioCreado = await _login.CrearUsuarioSeguridad(user);

            return Ok($"se creo correctamente el usuario {usuarioCreado.Nombre} {usuarioCreado.Usuario}");


            
        }
    }
}