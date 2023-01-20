using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using UserManager.DTO;

namespace UserManager.Repositorios
{

    public interface ILogin
    {
        public Task<string> Loguear(LoginDTO user);
        public Task<CrearUsuarioDTO> CrearUsuarioSeguridad(CrearUsuarioDTO user,IDbConnection db);
        public Task<LoginDTO> ObtenerUsuarioLogin(string user, IDbConnection db);
        public Task<LoginDTO> ObtenerUsuarioLoginDB(string user);
    }

    public class Login : ILogin
    {
        private readonly IConfiguration _config;
        private readonly IPasswordHasherRepositorio _passwordHash;

        public Login(IConfiguration config, IPasswordHasherRepositorio passwordHash)
        {
            _config = config;
            _passwordHash = passwordHash;
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

            if (!(_passwordHash.CheckHash(loginUser.Contraseña, user.Contraseña)))
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
                Subject = new ClaimsIdentity(new[] { new Claim("USUARIO", user.Usuario),new Claim("LEGAJO", user.Contraseña) }),
                Expires = DateTime.UtcNow.AddHours(1),
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

            var passHash = _passwordHash.Hash(user.Contraseña);

            string sql = @"INSERT INTO T_USUARIO_LOGIN (USUARIO,NOMBRE,CONTRASEÑA,MAIL,ACTIVO) VALUES (@usuario,@nombre,@contraseña,@mail,@activo)";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("usuario", user.Usuario, DbType.String);
            dp.Add("nombre", user.Nombre, DbType.String);
            dp.Add("contraseña", passHash, DbType.String);
            dp.Add("mail",user.Mail,DbType.String);
            dp.Add("activo",1,DbType.Int16);


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
    }
}