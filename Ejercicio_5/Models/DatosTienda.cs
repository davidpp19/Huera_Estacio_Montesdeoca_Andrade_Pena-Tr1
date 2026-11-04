using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ejercicio_5.Models
{
    public class DatosTienda
    {
        public List<Artesano> Artesanos { get; set; } = new List<Artesano>();

        public List<Artesania> Artesanias { get; set; } = new List<Artesania>();

        public List<LotesConsignacion> Lotes { get; set; } = new List<LotesConsignacion>();

        public List<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}
