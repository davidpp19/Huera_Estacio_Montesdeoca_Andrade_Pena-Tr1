using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_4.Modelos
{
    public class RegistroVenta
    {
        public string nombreProducto { get; set; } 
        public int cantidadVendida { get; set; }
        public double totalCobrado { get; set; }
        public DateTime fechaVenta { get; set; }
    }
}
