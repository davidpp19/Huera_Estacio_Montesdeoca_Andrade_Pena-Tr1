using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_5.Models
{
    public enum TipoMovimiento
    {
        Recepcion,
        Venta,
        Devolucion
    }
    public class Movimiento
    {
        public int Id { get; set; }
        public int ArtesaniaId { get; set; }
        public int ArtesanoId { get; set; }
        public int LoteId { get; set; }

        public TipoMovimiento Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }
        public decimal ComisionTotal { get; set; }

        public decimal ObtenerTotalVenta()
        {
            if (Tipo == TipoMovimiento.Venta)
            {
                return PrecioUnitario * Cantidad;
            }

            return 0;
        }

        public decimal ObtenerPagoArtesano()
        {
            return ObtenerTotalVenta() - ComisionTotal;
        }
    }
}
