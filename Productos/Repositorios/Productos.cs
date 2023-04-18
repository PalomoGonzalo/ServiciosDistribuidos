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
        Task<ProductoDTO> ObtenerProductoPorId(int id);
        Task<ProductoEliminarDTO> ObtenerProductoPorId(int id,IDbConnection db);
        Task<IEnumerable<ProductoDTO>> ObtenerProductosPorNombre(string nombre);
        
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
            
            IEnumerable<ProductosPaginacioDTO> lista = await db.QueryAsync<ProductosPaginacioDTO>(sql,dp).ConfigureAwait(false);

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

        /// <summary>
        /// SE obtiene los productos por id en la tabla productos
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<ProductoDTO> ObtenerProductoPorId(int id)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string sql = $"SELECT * FROM PRODUCTOS WHERE ID_PRODUCTO = @ID";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("ID",id,DbType.Int32);

            ProductoDTO producto = await db.QueryFirstOrDefaultAsync<ProductoDTO>(sql,dp).ConfigureAwait(false);

            if(producto == null)
            {
                throw new Exception("El producto no existe");
            }

            return producto;
        }



        public async Task<ProductoEliminarDTO> ObtenerProductoPorId(int id,IDbConnection db)
        {
            string sql = $"SELECT * FROM PRODUCTOS WHERE ID_PRODUCTO = @ID";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("ID",id,DbType.Int32);

            ProductoEliminarDTO producto = await db.QueryFirstOrDefaultAsync<ProductoEliminarDTO>(sql,dp).ConfigureAwait(false);

            if(producto == null)
            {
                throw new Exception("El producto no existe");
            }

            return producto;
        }


        /// <summary>
        /// Se obtiene lista de productos por nombre 
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProductoDTO>> ObtenerProductosPorNombre(string nombre)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            string sql = $@"select * from productos where nombre like concat ('%',@test,'%')";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("test",nombre,DbType.String);
            
            IEnumerable<ProductoDTO> productosConNombre = await db.QueryAsync<ProductoDTO>(sql,dp).ConfigureAwait(false);

            if(productosConNombre.Count() == 0)
            {
                throw new Exception("No existe productos con ese nombre");
            }

            return productosConNombre;
        }


        public async Task<int> DarDeBajaProductoLogico(int idProducto)
        {
            
            using IDbConnection db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            if (db.State is ConnectionState.Closed) db.Open();


            ProductoEliminarDTO productoAEliminar = await this.ObtenerProductoPorId(idProducto,db);

            if(productoAEliminar==null)
            {
                throw new Exception("Error en la cosnulta de producto");
            }

            if()
            using IDbTransaction transaccion = db.BeginTransaction();


        }


        public async Task<int> DarDeBajaProductoLogico(ProductoDTO idProducto, IDbConnection db)
        {
            





        }
    }
}