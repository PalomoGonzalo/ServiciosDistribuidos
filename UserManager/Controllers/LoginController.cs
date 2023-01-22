using System.Reflection;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserManager.DTO;
using UserManager.Repositorios;
using Microsoft.AspNetCore.Authorization;

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

            // SecurityToken encodeJwt = tokenhandler.ReadJwtToken(token);
            //var tokenLectura = new JwtSecurityTokenHandler().ReadJwtToken(token);
            //var claim = tokenLectura.Claims.ToString();

            return Ok(token);
        }
        /// <summary>
        /// Crear contraseña
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("RegistrarUsuario")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] CrearUsuarioDTO user)
        {
            CrearUsuarioDTO usuario =await _usuario.RegistrarUsuario(user);

            if(usuario==null)
            {
                return NotFound("El usuario ya existe");
            }

            return Ok($"se creo correctamente el usuario {usuario.Nombre} {usuario.Usuario}");
        }
        
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

        [HttpGet("Header")]
        [Authorize]
        public IActionResult GetHeaders()
        {
            string test = Request.Headers.Authorization;
            string  []strlist = test.Split("Bearer ",StringSplitOptions.RemoveEmptyEntries);
            test = String.Join("",strlist);

            var tokenLectura = new JwtSecurityTokenHandler().ReadJwtToken(test);
            string nombre = tokenLectura.Claims.Where(x=>x.Type=="USUARIO").Select(c=>c.Value).SingleOrDefault();
            string legajo = tokenLectura.Claims.Where(x=>x.Type=="LEGAJO").Select(c=>c.Value).SingleOrDefault();

            LoginDTO user = new LoginDTO {
                Usuario = nombre,
                Password = legajo
            };
            return  Ok(user);
        } 
               
        [HttpPut("CambiarContraseña")]
        public async Task<IActionResult> CambiarContraseña ([FromBody] CambiarContraseñaDTO user)
        {
            if (await _login.CambiarContraseña(user))
            {
                return Ok("Se cambio correctamente la contraseña ");
            }

            return BadRequest("Error no coincide la contraseña");
        }





    }
}