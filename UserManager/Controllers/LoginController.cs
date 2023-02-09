using System.Reflection;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserManager.DTO;
using UserManager.Repositorios;
using UserManager.Types;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace UserManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogin _login;
        private readonly IUsuario _usuario;

        public LoginController(ILogin login, IUsuario usuario = null)
        {
            _login = login;
            _usuario = usuario;
        }

        /// <summary>
        /// Loguea y devuelve un token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPost("Loguear")]
        public async Task<IActionResult> Loguear([FromBody] LoginDTO user)
        {
            //JwtSecurityTokenHandler tokenhandler = new JwtSecurityTokenHandler();

            if (user == null)
                return BadRequest("Error datos no validos");
            string token = await _login.Loguear(user);

            if (token == null)
            {
                return BadRequest("error en el usuario o password ");
            }

            LoginDTO id = await _login.ObtenerUsuarioLoginDB(user.Usuario);

            // SecurityToken encodeJwt = tokenhandler.ReadJwtToken(token);
            //var tokenLectura = new JwtSecurityTokenHandler().ReadJwtToken(token);
            //var claim = tokenLectura.Claims.ToString();

            return Ok(new { Token = token, Legajo = id.Legajo });
        }
        /// <summary>
        /// Registra un usuario en t_usuario_lgoin y tambien inserta datos en la tabla t_usuario
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("RegistrarUsuario")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] CrearUsuarioDTO user)
        {
            try
            {
                CrearUsuarioDTOResponse usuario = await _usuario.RegistrarUsuario(user);
                return Ok(new HttpResponseOk { data = usuario, msg = "Se creo correctamente el usuario" });

            }
            catch (Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }

        }
        /// <summary>
        /// prubeas de headers con los tokens
        /// </summary>
        /// <returns></returns>

        [HttpGet("Headers")]
        [Authorize]
        public ActionResult<Dictionary<string, string>> GetAllHeaders()
        {
            Dictionary<string, string> requestHeaders =
               new Dictionary<string, string>();
            foreach (var header in Request.Headers)
            {
                requestHeaders.Add(header.Key, header.Value);
            }
            
            
            return requestHeaders;
        }
        /// <summary>
        /// prubeas de headers con los tokens
        /// </summary>
        /// <returns></returns>
        [HttpGet("Header")]
        [Authorize]
        public IActionResult GetHeaders()
        {
            string test = Request.Headers.Authorization;
            string[] strlist = test.Split("Bearer ", StringSplitOptions.RemoveEmptyEntries);
            test = String.Join("", strlist);

            var tokenLectura = new JwtSecurityTokenHandler().ReadJwtToken(test);
            string nombre = tokenLectura.Claims.Where(x => x.Type == "USUARIO").Select(c => c.Value).SingleOrDefault();
            string legajo = tokenLectura.Claims.Where(x => x.Type == "LEGAJO").Select(c => c.Value).SingleOrDefault();

            LoginDTO user = new LoginDTO
            {
                Usuario = nombre,
                Password = legajo
            };

            return Ok (user);

        }
        /// <summary>
        /// Cambia la contraseña, verificando que ingrese la anterior correctamente
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPut("CambiarContraseña")]
        public async Task<IActionResult> CambiarContraseña([FromBody] CambiarContraseñaDTO user)
        {
            try
            {
                await _login.CambiarContraseña(user);

                return Ok(new HttpResponseOk { msg = "Se cambio correctamente la contraseña" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }

        /// <summary>
        /// Se cambia de contraseña de cualquer usuario solo el administrador lo puede realizar
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        
        [HttpPut("CambiarContraseñaForzada")]
        [Authorize]
        public async Task<IActionResult> CambiarContraseñaForzada([FromBody] CambiarContraseñaForzadaDTO user)
        {
            try
            {
                await _login.CambiarContraseñaForzada(user);

                return Ok(new HttpResponseOk { msg = "Se cambio correctamente la contraseña" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new HttpBadResponse(ex));
            }
        }

    }
}