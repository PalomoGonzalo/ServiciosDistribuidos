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
    }

    public class Usuario : IUsuario
    {
        private readonly IConfiguration _config;

        public Usuario(IConfiguration config)
        {
            _config = config;
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
    }
}