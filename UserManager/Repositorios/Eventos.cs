using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using UserManager.DTO;

namespace UserManager.Repositorios
{

    public interface IEventos
    {
        Task <int> InsertarEventoRegistrarUsuario(CrearUsuarioDTO user,string usuarioQuienRealiza);

    }
    public class Eventos : IEventos
    {

        private readonly IConfiguration _config;

        public Eventos(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> InsertarEventoRegistrarUsuario(CrearUsuarioDTO user, string usuarioQuienRealiza)
        {
            using IDbConnection db = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

            EventoDTO evento = new EventoDTO();

            string registrarUsuario = "RegitrarUsuario";

            string usuario2 =JsonConvert.SerializeObject(user);

            string sql = $@"INSERT INTO T_EVENTOS (ID_EVENTO, EVENTONOMBRE, USUARIO) VALUES (@id_evento,@eventonombre,@usuario)";

            DynamicParameters dp = new DynamicParameters();

            dp.Add("id_evento",1,DbType.Int16);
            dp.Add("eventonombre",registrarUsuario,DbType.String);
            dp.Add("usuario",usuarioQuienRealiza,DbType.String);
           

            int row = await db.ExecuteAsync(sql,db);
            return row;

        }
    }
}