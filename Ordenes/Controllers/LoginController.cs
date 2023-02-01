using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordenes.DTO;
using Ordenes.Repositorio;

namespace Ordenes.Controllers
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



        [HttpPost("Loguear")]
        public async Task<IActionResult> Loguear ([FromBody] LoginDTO login)
        {
            try
            {
                TokenDTO token = await _login.Loguear(login);
                return Ok(token);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult test ()
        {
            return Ok("test");
        }

    }
}