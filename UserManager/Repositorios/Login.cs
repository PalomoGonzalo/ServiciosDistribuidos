using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using UserManager.DTO;
using UserManager.Helpers;

namespace UserManager.Repositorios
{

    public interface ILogin
    {
        public Task<string> Loguear(LoginDTO user);
        public Task<CrearUsuarioDTO> CrearUsuarioSeguridad(CrearUsuarioDTO user, IDbConnection db);
        public Task<LoginDTO> ObtenerUsuarioLogin(string user, IDbConnection db);
        public Task<LoginDTO> ObtenerUsuarioLoginDB(string user);
        public Task<Boolean> CambiarContraseña(CambiarContraseñaDTO contraseñaDTO);
        public Task<Boolean> CambiarContraseñaForzada(CambiarContraseñaForzadaDTO contraseñaDTO);
    }

    public class Login : ILogin
    {
        private readonly IConfiguration _config;
        private readonly IPasswordHasherRepositorio _passwordHash;
        private readonly IEventos _evento;
        private readonly ICliente _cliente;
        private readonly IHttpContextAccessor http;


        public Login(IConfiguration config, IPasswordHasherRepositorio passwordHash, IEventos evento, ICliente cliente, IHttpContextAccessor http)
        {
            _config = config;
            _passwordHash = passwordHash;
            _evento = evento;
            _cliente = cliente;
            this.http = http;
        }

        /// <summary>
        /// Se obtiene el usuario y contraseña, checkea el hash 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>el toke  o null</returns>
        public async Task<string> Loguear(LoginDTO user)
        {
            LoginDTO loginUser = await ObtenerUsuarioLoginDB(user.Usuario);

            if (loginUser == null)
            {
                return null;
            }

            if (!(_passwordHash.CheckHash(loginUser.Password, user.Password)))
            {
                return null;
            }

            string token = GenerarToken(loginUser);
            return token;
        }
        /// <summary>
        /// Genera token jwt que expira en una hora
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public String GenerarToken(LoginDTO user)
        {
            JwtSecurityTokenHandler tokenhandler = new JwtSecurityTokenHandler();

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("USUARIO", user.Usuario), new Claim("LEGAJO", user.Legajo.ToString()),new Claim("IP",_cliente.ObtenerIpCliente(http)) }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:SecretKey"])), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = tokenhandler.CreateToken(tokenDescriptor);
            String encodeJwt = tokenhandler.WriteToken(token);

            return encodeJwt;
        }
        /// <summary>
        /// Crea un usuario y contraseña en la tabla t_usuario_login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<CrearUsuarioDTO> CrearUsuarioSeguridad(CrearUsuarioDTO user, IDbConnection db)
        {
            if (user == null)
                throw new ArgumentNullException();

            var passHash = _passwordHash.Hash(user.Password);

            string sql = @$"INSERT INTO T_USUARIO_LOGIN (USUARIO,NOMBRE,PASSWORD,MAIL,ACTIVO) VALUES (@usuario,@nombre,@contraseña,@mail,@activo)";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("usuario", user.Usuario, DbType.String);
            dp.Add("nombre", user.Nombre, DbType.String);
            dp.Add("contraseña", passHash, DbType.String);
            dp.Add("mail", user.Mail, DbType.String);
            dp.Add("activo", Types.EnumsLib.TipoEstado.Activo, DbType.Int16);


            int row = await db.ExecuteAsync(sql, dp);

            if (row == 0)
            {
                throw new Exception("No se logro crear correctamente el usuario");
            }
            return user;
        }
        /// <summary>
        /// Obtiene Un usuario Por el nombre del usuario, en una instancia de db ya creada
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<LoginDTO> ObtenerUsuarioLogin(string user, IDbConnection db)
        {
            string query = $@"SELECT * FROM T_USUARIO_LOGIN WHERE USUARIO=@user";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("user", user, DbType.String);

            return await db.QueryFirstOrDefaultAsync<LoginDTO>(query, dp);
        }
        /// <summary>
        /// Obtiene Un usuario Por el nombre del usuario creando una instancia a la bd
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<LoginDTO> ObtenerUsuarioLoginDB(string user)
        {
            using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            string query = $@"SELECT * FROM T_USUARIO_LOGIN WHERE USUARIO=@user";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("user", user, DbType.String);

            return await db.QueryFirstOrDefaultAsync<LoginDTO>(query, dp);
        }
        /// <summary>
        /// Metodo que cambia contraseña verificando la contraseña que ingresa el usuario,
        /// se obtiene por usuario la tabla del usario y se desencripta con la contraseña que se verifico
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Boolean> CambiarContraseña(CambiarContraseñaDTO contraseñaDTO)
        {
            LoginDTO userCheckPass = await this.ObtenerUsuarioLoginDB(contraseñaDTO.Usuario);
            if (userCheckPass == null)
            {
                throw new Exception("Error usuario invalido");
            }
            if (_passwordHash.CheckHash(userCheckPass.Password, contraseñaDTO.Password))
            {
                string passHasheada = _passwordHash.Hash(contraseñaDTO.PasswordNueva);

                using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
                if (db.State is ConnectionState.Closed) db.Open();

                using IDbTransaction transaccion = db.BeginTransaction();
                string sql = "UPDATE T_USUARIO_LOGIN SET PASSWORD = @contraseña where LEGAJO = @legajo";

                DynamicParameters dp = new DynamicParameters();
                dp.Add("contraseña", passHasheada, DbType.String);
                dp.Add("legajo", contraseñaDTO.Legajo, DbType.Int32);
                int row = await db.ExecuteAsync(sql, dp);
                if (row <= 0)
                {
                    throw new Exception("Error al cambiar la contraseña, legajo no coinciden");
                }
                transaccion.Commit();
            }
            else
            {
                throw new Exception("Error la contraseña no coinciden");
            }

            return true;
        }
        /// <summary>
        /// Se cambia de contraseña sin ningun tipo de validacion 
        /// </summary>
        /// <param name="contraseñaDTO"></param>
        /// <returns></returns>
        public async Task<Boolean> CambiarContraseñaForzada(CambiarContraseñaForzadaDTO contraseñaDTO)
        {
            try
            {
                string passHasheada = _passwordHash.Hash(contraseñaDTO.PasswordNueva);

                using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
                if (db.State is ConnectionState.Closed) db.Open();

                using IDbTransaction transaccion = db.BeginTransaction();


                string sql = "UPDATE T_USUARIO_LOGIN SET PASSWORD = @contraseña where LEGAJO = @legajo";

                DynamicParameters dp = new DynamicParameters();
                dp.Add("contraseña", passHasheada, DbType.String);
                dp.Add("legajo", contraseñaDTO.Legajo, DbType.Int32);
                int row = await db.ExecuteAsync(sql, dp);

                EventoCambiarContraseñaDTO eventoUser = new EventoCambiarContraseñaDTO{
                    Legajo = contraseñaDTO.Legajo
                };

                int rowEvento = await _evento.InsertarEvento(eventoUser,db,"CambiarContraseñaForzada",((int)Types.EnumsLib.EventosEstados.EventoCambiarContraseñaForzada));
                if (row <= 0 && rowEvento <= 0) 
                {
                    transaccion.Rollback();
                    throw new Exception("Error al cambiar la contraseña");
                }

                transaccion.Commit();
                return true;

            }
            catch (System.Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}