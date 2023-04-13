using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productos.DTOS
{
    public class ProductoDTO
    {
        public int Id_Producto { get; set; }
        public string Descripcion { get; set; }
        public double Precio { get; set; }
        public double Stock { get; set; }
        public int CategoriaID { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public DateTime Fecha_modificacion { get; set; }
        
    }
}