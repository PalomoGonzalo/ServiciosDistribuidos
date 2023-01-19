using System.Reflection;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        /// <summary>
        /// Loguea y devuelve un token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO user)
        {
            //JwtSecurityTokenHandler tokenhandler = new JwtSecurityTokenHandler();
            if (user == null)
                return BadRequest("Error datos no validos");
            string token = await _login.Loguear(user);

            if (token == null)
            {
                return BadRequest("error en el usuario o contrase�a ");
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
        [HttpPost("CrearContraseña")]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDTO user)
        {
            LoginDTO usuarioExiste = await _login.ObtenerUsuarioLogin(user.Usuario);

            if (usuarioExiste != null)
            {
                return BadRequest($"El usario {user.Usuario} ya existe");
            }
            CrearUsuarioDTO usuarioCreado = await _login.CrearUsuarioSeguridad(user);

            return Ok($"se creo correctamente el usuario {usuarioCreado.Nombre} {usuarioCreado.Usuario}");
        }

        [HttpGet("Headers")]
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
        public async Task<IActionResult> GetHeaders()
        {
            string test = Request.Headers.Authorization;
            string  []strlist = test.Split("Bearer ",StringSplitOptions.RemoveEmptyEntries);
            test = String.Join("",strlist);

            var tokenLectura = new JwtSecurityTokenHandler().ReadJwtToken(test);
            string nombre = tokenLectura.Claims.Where(x=>x.Type=="USUARIO").Select(c=>c.Value).SingleOrDefault();
            string legajo = tokenLectura.Claims.Where(x=>x.Type=="LEGAJO").Select(c=>c.Value).SingleOrDefault();

            LoginDTO user = new LoginDTO {
                Usuario = nombre,
                Contraseña = legajo
            };
            return  Ok(user);
        } 

    }
}