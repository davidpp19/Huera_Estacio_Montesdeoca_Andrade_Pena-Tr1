using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_5.Models
{
    public class LotesConsignacion
    {
        public int Id { get; set; }
        public int ArtesaniaId { get; set; }
        public DateTime FechaRecepcion { get; set; }

        public int CantidadRecibida { get; set; }
        public int CantidadDisponible { get; set; }
        public int CantidadVendida { get; set; }
        public int CantidadDevuelta { get; set; }

        public void Vender(int cantidad)
        {
            if (cantidad <= 0 || cantidad > CantidadDisponible)
            {
                throw new InvalidOperationException("No hay suficiente cantidad disponible para vender.");
            }
            CantidadDisponible -= cantidad;
            CantidadVendida += cantidad;
        }

        public void Devolver(int cantidad)
        {
            if (cantidad <= 0 || cantidad > CantidadDisponible)
            {
                throw new InvalidOperationException("No hay suficiente cantidad disponible para devolver.");
            }
            CantidadDisponible -= cantidad;
            CantidadDevuelta += cantidad;
        }

        public bool DebeDevolverse()
        {
            int dias = (DateTime.Now - FechaRecepcion).Days;
            
            return dias > 90 && CantidadDisponible > 0;
        }

        public bool VerificarInventario()
        {
            return CantidadRecibida == CantidadDisponible + CantidadVendida + CantidadDevuelta;
        }
    }
}
