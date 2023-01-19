using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using UserManager.DTO;

namespace UserManager.Repositorios
{

    public interface IUsuario
    {
        public Task<UsuarioDTO> ObtenerUsuarioPorLegajo(int legajo);
        public Task<IEnumerable<UsuarioDTO>> ObtenerTodosLosUsuarios();
        public Task<UsuarioDTO> ObtenerUsuarioPorDni(int dni);
        public Task InsertarRegistrarseEnUsuario(CrearUsuarioDTO login);
    }

    public class Usuario : IUsuario
    {
        private readonly IConfiguration _config;
        private readonly ILogin _login;

        public Usuario(IConfiguration config, ILogin login)
        {
            _config = config;
            _login = login;
        }
        /// <summary>
        /// Se obtiene usuario por legajo
        /// </summary>
        /// <param name="legajo"></param>
        /// <returns></returns>
        public async Task<UsuarioDTO> ObtenerUsuarioPorLegajo(int legajo)
        {
            using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

            string query = $@"SELECT * FROM T_USUARIO WHERE LEGAJO=@user";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("user", legajo, DbType.Int64);

            return await db.QueryFirstOrDefaultAsync<UsuarioDTO>(query, dp);
        }
        /// <summary>
        /// Se obtiene un usuario por dni 
        /// </summary>
        /// <param name="dni"></param>
        /// <returns></returns>
        public async Task<UsuarioDTO> ObtenerUsuarioPorDni(int dni)
        {
            using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

            string query = $@"SELECT * FROM T_USUARIO WHERE DNI=@user";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("user", dni, DbType.Int64);

            return await db.QueryFirstOrDefaultAsync<UsuarioDTO>(query, dp);
        }
        /// <summary>
        /// Se obtiene todos los usuarios que esten activos
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UsuarioDTO>> ObtenerTodosLosUsuarios()
        {
            using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

            string query = $@"SELECT * FROM T_USUARIO where ACTIVO = 1";

            IEnumerable<UsuarioDTO> listaUsuario = await db.QueryAsync<UsuarioDTO>(query).ConfigureAwait(false);

            return listaUsuario;
        }


        public async Task InsertarRegistrarseEnUsuario(CrearUsuarioDTO login)
        {
            LoginDTO legajo =  await _login.ObtenerUsuarioLogin(login.Usuario);

            using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

            string query = $@"INSERT INTO T_USUARIO (LEGAJO,NOMBRE,MAIL,ROL,ACTIVO) VALUES (@legajo, @nombre, @mail, @rol,@activo)";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("legajo",legajo.Legajo,DbType.Int32);
            dp.Add("nombre",login.Nombre,DbType.String);
            dp.Add("mail",login.Mail,DbType.String);
            dp.Add("rol",1, DbType.Int16);
            dp.Add("activo",1,DbType.Int16);

            int row = await db.ExecuteAsync(query, dp);
            if (row == 0)
            {
                throw new Exception("No se logro crear correctamente el usuario");
            }

        }
    }
}