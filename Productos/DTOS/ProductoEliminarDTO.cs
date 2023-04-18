using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productos.DTOS
{
    public class ProductoEliminarDTO
    {
        public int Id_Producto { get; set; }
        public string Descripcion { get; set; }
        public int Activo { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public DateTime Fecha_modificacion { get; set; }
    }
}