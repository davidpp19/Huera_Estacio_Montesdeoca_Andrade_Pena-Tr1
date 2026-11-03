using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_5.Models
{
    public enum TipoArtesania
    {
        Textil,
        TallaMadera
    }

    public class Artesania
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public TipoArtesania Tipo { get; set; }
        public decimal PrecioVenta { get; set; }
        public int ArtesanoId { get; set; }

        public decimal ObtenerComision()
        {
            if (Tipo == TipoArtesania.Textil)
            {
                return PrecioVenta * 0.20m;
            }
            else
            {
                return PrecioVenta * 0.25m;
            }
        }
    }
}