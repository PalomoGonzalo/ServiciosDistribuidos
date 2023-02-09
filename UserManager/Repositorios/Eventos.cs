using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using UserManager.DTO;
using UserManager.Helpers;

namespace UserManager.Repositorios
{

    public interface IEventos
    {
        Task<int> InsertarEvento(object user, string usuarioQuienRealiza, IDbConnection db,IHttpContextAccessor http,string eventoNombre,int idEvento);
        public Task<IEnumerable<EventoDTO>> ObtenerEventosPorIdEvento(int idEvento);
    }
    public class Eventos : IEventos
    {
        private readonly IConfiguration _config;
        private readonly IMapper _maper;
        private readonly ICliente _cliente;

        public Eventos(IConfiguration config, IMapper maper, ICliente cliente)
        {
            _config = config;
            _maper = maper;
            _cliente = cliente;
        }

        /// <summary>
        /// Cuando se registra un usuario se guarda en la tabla t_eventos los datos de que se registro con el id = 1
        /// </summary>
        /// <param name="user"></param>
        /// <param name="usuarioQuienRealiza"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<int> InsertarEvento(object user, string usuarioQuienRealiza, IDbConnection db,IHttpContextAccessor http, string eventoNombre,int idEvento)
        {
            
            string usuarioObject = JsonConvert.SerializeObject(user);

            string userIP = _cliente.ObtenerIpCliente(http);

            string dateTimeStamp = _cliente.ObtenerDateTimeNowFormatoTimeStamp();

            string sql = $@"INSERT INTO T_EVENTOS (ID_EVENTO, EVENTONOMBRE, USUARIO, DATO, FECHA_CREACION, IP) VALUES (@id_evento,@eventonombre,@usuario,@dato,@fecha,@ip)";

            DynamicParameters dp = new DynamicParameters();

            dp.Add("id_evento", idEvento, DbType.Int16);
            dp.Add("eventonombre", eventoNombre, DbType.String);
            dp.Add("usuario", usuarioQuienRealiza, DbType.String);
            dp.Add("dato", usuarioObject, DbType.String);
            dp.Add("fecha",dateTimeStamp,DbType.String);
            dp.Add("ip",userIP,DbType.String);

            int row = await db.ExecuteAsync(sql, dp);
            
            return row;
        }

        /// <summary>
        ///  se conecta a la base de datos se obtiene  datos de la tabla t_eventos
        /// </summary>
        /// <param name="idEvento"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EventoDTO>> ObtenerEventosPorIdEvento(int idEvento)
        {
            using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

            string sql = $@"SELECT * FROM T_EVENTOS WHERE ID_EVENTO = @id";

            DynamicParameters dp = new DynamicParameters();

            dp.Add("id", idEvento, DbType.Int16);

            IEnumerable<EventoDTO> eventos = await db.QueryAsync<EventoDTO>(sql, dp).ConfigureAwait(false);

            if (eventos == null)
            {
                throw new Exception(@$"No hay datos para el id {idEvento}");
            }

            foreach (var item in eventos)
            {
                if(item.Dato != null)
                    item.DatoUsuario = JsonConvert.DeserializeObject<ExpandoObject>(item.Dato);
            }
            
            return eventos;
        }
    }
}