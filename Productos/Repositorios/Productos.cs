using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Productos.DTOS;

namespace Productos.Repositorios
{
    public interface IProductos
    {
        Task<IEnumerable<ProductosPaginacioDTO>> ObtenerTodosLosProductosPorPagina(int nroPagina);
        Task<int> ObtenerCantidadDeRegistrosPaginacion(IDbConnection db);
        
    }

    public class Producto : IProductos
    {
        private readonly IConfiguration _config;

        public Producto(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Se obtiene datos del producto ejucutando un sp
        /// </summary>
        /// <param name="nroPagina"> sobre la cantidad de registros el numero de pagina</param>
        /// <param name="cantidadDeRegistrosATraer"> cantidad de registro que se quiere mostrar</param>
        /// <returns></returns>
        public async Task<IEnumerable<ProductosPaginacioDTO>> ObtenerTodosLosProductosPorPagina(int nroPagina)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            int cantidadDeRegistrosATraer = await this.ObtenerCantidadDeRegistrosPaginacion(db);

            string sql = $@"exec spPaginado @pagina,@cantidadRegistros";

            DynamicParameters dp = new DynamicParameters();

            dp.Add("pagina",nroPagina,DbType.Int32);
            dp.Add("cantidadRegistros",cantidadDeRegistrosATraer,DbType.Int32);
            
            IEnumerable<ProductosPaginacioDTO> lista = await db.QueryAsync<ProductosPaginacioDTO>(sql,dp,commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return lista;
        }
        /// <summary>
        /// Obtiene cantidad de registro por pagina a mostrar parametrizado por catalogo
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<int> ObtenerCantidadDeRegistrosPaginacion(IDbConnection db)
        {
            string sql = $"select DATA from parametros where ID_PARAMETRO = 1";
            String data = await db.QuerySingleAsync<String>(sql).ConfigureAwait(false);
            if(data == null)
            {
                throw new Exception("Error no existe datos de paginacion");
            }
            CantidadDeRegistrosPaginacionDTO cantidadDeRegistros = JsonConvert.DeserializeObject<CantidadDeRegistrosPaginacionDTO>(data);
            return cantidadDeRegistros.NroDeRegistros;
        }

        public async Task<int> ObtenerProductosPorId(int id)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string sql = $"SELECT * FROM PRODUCTOS WHERE ID_PRODUCTO = @ID";
            DynamicParameters dp = new DynamicParameters();
            dp.Add()







        }
    }
}