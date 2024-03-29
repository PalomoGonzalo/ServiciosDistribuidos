using System.Data.Common;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using UserManager.DTO;
using System.IdentityModel.Tokens.Jwt;
using static UserManager.Types.EnumsLib;
using UserManager.Helpers;
using UserManager.Types;

namespace UserManager.Repositorios
{

    public interface IUsuario
    {
        public Task<UsuarioDTO> ObtenerUsuarioPorLegajo(int legajo);
        public Task<IEnumerable<UsuarioDTO>> ObtenerTodosLosUsuarios();
        public Task<UsuarioDTO> ObtenerUsuarioPorDni(int dni);
        public Task<int> InsertarRegistrarseEnUsuario(CrearUsuarioDTO login, IDbConnection db);
        public Task<CrearUsuarioDTOResponse> RegistrarUsuario(CrearUsuarioDTO user);
        public Task<CambiarDatosUsuarioDTO> CambiarDatosUsuario(CambiarDatosUsuarioDTO user);
        public Task<UsuarioBajaDTO> DarDeBajaLogicaUsuario(UsuarioBajaDTO usuario);
        public Task<int> DarDeBajaUsuarioLogin(UsuarioDTO usuario, IDbConnection db);
        public Task<int> DarDeBajaUsuario(UsuarioDTO usuario, IDbConnection db);
    }

    public class Usuario : IUsuario
    {
        private readonly IConfiguration _config;
        private readonly ILogin _login;
        private readonly IEventos _eventos;
        private readonly ICliente _cliente;
        private readonly IMapper _mapper;

        public Usuario(IConfiguration config, ILogin login, IEventos eventos = null, IMapper mapper = null, ICliente cliente = null)
        {
            _config = config;
            _login = login;
            _eventos = eventos;
            _mapper = mapper;
            _cliente = cliente;
           
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

            UsuarioDTO user = await db.QueryFirstOrDefaultAsync<UsuarioDTO>(query, dp);

            if (user == null)
            {
                throw new Exception("Error, el usuario no existe");
            }
            return user;

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

            if (listaUsuario == null)
            {
                throw new Exception("Error al traer los usuarios");
            }

            return listaUsuario;
        }

        /// <summary>
        /// Inserta en la taba t_usuario al registrar cono los datos obtenidos de la tabla t_usuario_login
        /// </summary>
        /// <param name="login"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<int> InsertarRegistrarseEnUsuario(CrearUsuarioDTO login, IDbConnection db)
        {
            LoginDTO legajo = await _login.ObtenerUsuarioLogin(login.Usuario, db);

            string query = $@"INSERT INTO T_USUARIO (LEGAJO,NOMBRE,MAIL,ROL,ACTIVO) VALUES (@legajo, @nombre, @mail, @rol,@activo)";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("legajo", legajo.Legajo, DbType.Int32);
            dp.Add("nombre", login.Nombre, DbType.String);
            dp.Add("mail", login.Mail, DbType.String);
            dp.Add("rol", 1, DbType.Int16);
            dp.Add("activo", Types.EnumsLib.TipoEstado.Activo, DbType.Int16);

            int row = await db.ExecuteAsync(query, dp);
            if (row == 0)
            {
                throw new Exception("No se logro crear correctamente el usuario");
            }
            return row;

        }

        /// <summary>
        /// Registra un usuario, como se hace parcialemente primero se controla que no que el usuario exista,
        /// se guarda los datos en t_usuario_login y por ultimo guarda en la tabla t_usuario
        /// En caso de tener algun tipo de error se realiza un rollback
        /// Si sale todo bien se realiza el comit
        /// /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<CrearUsuarioDTOResponse> RegistrarUsuario(CrearUsuarioDTO user)
        {
            int row = 0;
            using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            if (db.State is ConnectionState.Closed) db.Open();

            using IDbTransaction transaccion = db.BeginTransaction();

            LoginDTO usuarioExiste = await _login.ObtenerUsuarioLogin(user.Usuario, db);

            if (usuarioExiste != null)
            {
                throw new Exception($"Error el usuario ya existe");
            }
            CrearUsuarioDTO usuarioCreado = await _login.CrearUsuarioSeguridad(user, db);

            if (usuarioCreado == null)
            {
                throw new Exception($"Error al crear el usuario");
            }
            

            row = await this.InsertarRegistrarseEnUsuario(usuarioCreado, db);
            
            EventoRegistrarUsuarioDTO userioEvento = _mapper.MapCrearUsuarioAEventoRegistrarUsuario(user);


            int rowEvento = await _eventos.InsertarEvento(userioEvento, db,"RegitrarUsuario",((int)EnumsLib.EventosEstados.EventoReprogramado));

            if (row == 0 && rowEvento == 0)
            {
                transaccion.Rollback();
                throw new Exception($"Error Al Insertar el evento");
            }
            transaccion.Commit();

            CrearUsuarioDTOResponse userResponse = new CrearUsuarioDTOResponse { Usuario = usuarioCreado.Usuario, Nombre = usuarioCreado.Nombre, Mail = usuarioCreado.Mail };
            return userResponse;

        }

        /// <summary>
        /// Se cambia los datos del usuario en la tabla t_usuario por legajo, pasandole todo los parametros aunque no lo vaya a cambiar
        /// Por que lo pisa
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<CambiarDatosUsuarioDTO> CambiarDatosUsuario(CambiarDatosUsuarioDTO user)
        {
            if (user == null)
            {
                throw new System.Exception("Error usuario es nulo");
            }

            UsuarioDTO userLegajo = await this.ObtenerUsuarioPorLegajo(user.Legajo);

            if (userLegajo == null)
            {
                throw new System.Exception("Error El usuario No existe");
            }

            using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            if (db.State is ConnectionState.Closed) db.Open();
            using IDbTransaction transaccion = db.BeginTransaction();

            string sql = "UPDATE T_USUARIO SET NOMBRE = @nombre, TELEFONO = @telefono, DIRECCION = @direccion, DNI = @dni where LEGAJO = @legajo";

            DynamicParameters dp = new DynamicParameters();

            dp.Add("nombre", user.Nombre, DbType.String);
            dp.Add("telefono", user.Telefono, DbType.String);
            dp.Add("direccion", user.Direccion, DbType.String);
            dp.Add("dni", user.Dni, DbType.String);
            dp.Add("legajo", user.Legajo, DbType.Int16);

            int row = await db.ExecuteAsync(sql, dp);
            if (row <= 0)
            {
                throw new Exception("Error al cambiar datos del usuario");
            }
            transaccion.Commit();
            return user;
        }


        /// <summary>
        /// Se da de baja al usuario poneniendo en activo 0, en t_usuario y t_usuario_login
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public async Task<UsuarioBajaDTO> DarDeBajaLogicaUsuario(UsuarioBajaDTO usuario)
        {
            if (usuario == null)
            {
                throw new Exception("El usuario es nullo");
            }

            UsuarioDTO usuarioExiste = await this.ObtenerUsuarioPorLegajo(usuario.Legajo);

            if (usuarioExiste == null)
            {
                throw new Exception("El usuario no existe");
            }

            using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            if (db.State is ConnectionState.Closed) db.Open();
            using IDbTransaction transaccion = db.BeginTransaction();

            int cambiosUsuarioLogin = await this.DarDeBajaUsuarioLogin(usuarioExiste, db);
            int cambiosUsuario = await this.DarDeBajaUsuario(usuarioExiste, db);


            int rowEvento = await _eventos.InsertarEvento(usuarioExiste,db,"DarDeBajaLogicaUsuario",((int)Types.EnumsLib.EventosEstados.EventoBajaLogica));

            if(cambiosUsuario > 1 && cambiosUsuarioLogin > 1)
            {
                transaccion.Rollback();
                throw new Exception("Error al realizar la baja logica");
            }

            transaccion.Commit();
            return usuario;
        }

        /// <summary>
        /// Se da de baja en la tabla t_usuario poniendolo en activo 0
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<int> DarDeBajaUsuario(UsuarioDTO usuario, IDbConnection db)
        {
            if (usuario == null)
            {
                throw new Exception("Usuario es nullo");
            }

            string sql = @$"UPDATE T_USUARIO SET ACTIVO = @baja WHERE LEGAJO = @legajo ";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("baja", TipoEstado.Baja, DbType.Int16);
            dp.Add("legajo", usuario.Legajo, DbType.Int32);

            int row = await db.ExecuteAsync(sql, dp);
            if (row == 0)
            {
                throw new Exception("No se logro realizar la baja");
            }
            if (row > 1)
            {
                throw new Exception("Error se realizo mas de 2 registros");
            }
            return row;
        }

        /// <summary>
        /// Se da de baja en la tabla t_usuario_login poniendolo en activo 0
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<int> DarDeBajaUsuarioLogin(UsuarioDTO usuario, IDbConnection db)
        {
            if (usuario == null)
            {
                throw new Exception("Usuario es nullo");
            }

            string sql = @$"UPDATE T_USUARIO_LOGIN SET ACTIVO = @baja WHERE LEGAJO = @legajo ";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("baja", TipoEstado.Baja, DbType.Int16);
            dp.Add("legajo", usuario.Legajo, DbType.Int32);

            int row = await db.ExecuteAsync(sql, dp);
            if (row == 0)
            {
                throw new Exception("No se logro realizar la baja");
            }
            if (row > 1)
            {
                throw new Exception("Error se realizo mas de 2 registros");
            }
            return row;
        }



    }
}