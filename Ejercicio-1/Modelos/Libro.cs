using Ejercicio_1.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_1.Modelos
{
    internal class Libro:Material
    {
        public Libro(String codigo, String titulo, int ejemplaresTotales) : base(codigo, titulo, ejemplaresTotales){ 
           
        }
        public override int diasPrestamo
        {
            get
            {
                return 7;
            }
        }
        public override decimal multaDiaria
        {
            get
            {
                return 0.25m;
            }
        }
    }
}
