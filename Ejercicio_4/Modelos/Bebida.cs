using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_4.Modelos
{
    public class Bebida
    {
        public string nombreBebida { get; set; }
        public int stockActual { get; set; } 
        public double precioFijo { get; set; }

        public double CalcularPrecioVenta()
        {
            double iva = precioFijo * 0.15;
            return precioFijo + iva;
        }
    }
}
