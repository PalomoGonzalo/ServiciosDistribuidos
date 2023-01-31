using System;
using System.Collections.Generic;
using System.Data;
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
        Task <int> InsertarEventoRegistrarUsuario(CrearUsuarioDTO user,string usuarioQuienRealiza,IDbConnection db);

    }
    public class Eventos : IEventos
    {

        private readonly IConfiguration _config;

        private readonly IMapper _maper;

        public Eventos(IConfiguration config, IMapper maper = null)
        {
            _config = config;
            _maper = maper;
        }

        public async Task<int> InsertarEventoRegistrarUsuario(CrearUsuarioDTO user, string usuarioQuienRealiza, IDbConnection db)
        {
            EventoRegistrarUsuarioDTO userioEvento = _maper.MapCrearUsuarioAEventoRegistrarUsuario(user);

            string usuario2 =JsonConvert.SerializeObject(userioEvento);

            string sql = $@"INSERT INTO T_EVENTOS (ID_EVENTO, EVENTONOMBRE, USUARIO, DATO) VALUES (@id_evento,@eventonombre,@usuario,@dato)";

            DynamicParameters dp = new DynamicParameters();

            dp.Add("id_evento",1,DbType.Int16);
            dp.Add("eventonombre","RegitrarUsuario",DbType.String);
            dp.Add("usuario",usuarioQuienRealiza,DbType.String);
            dp.Add("dato",usuario2,DbType.String);
           

            int row = await db.ExecuteAsync(sql,dp);
            return row;

        }
    }
}