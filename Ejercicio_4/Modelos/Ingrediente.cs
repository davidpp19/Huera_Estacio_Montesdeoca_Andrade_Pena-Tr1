using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_4.Modelos
{
    public class Ingrediente
    {
        public string nombreIngrediente { get; set; }
        public string unidadMedida { get; set; } 
        public double stockActual { get; set; }
        public double stockMinimo { get; set; }
        public double costoUnidad { get; set; }
        public bool formaParteDeReceta { get; set; } 

        public Ingrediente()
        {
            formaParteDeReceta = false;
        }
    }
}
