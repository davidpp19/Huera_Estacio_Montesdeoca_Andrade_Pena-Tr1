using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_4.Modelos
{
    public class Plato
    {
        public string nombrePlato { get; set; }
        public List<IngredienteReceta> receta { get; set; }

        public Plato()
        {
            receta = new List<IngredienteReceta>();
        }

        public double CalcularCosto()
        {
            double costoTotal = 0;
            foreach (IngredienteReceta ingrediente in receta)
            {
                costoTotal = costoTotal + (ingrediente.ingredienteBase.costoUnidad * ingrediente.cantidadUsada);
            }
            return costoTotal;
        }

        public double CalcularPrecioVenta()
        {
            double costo = CalcularCosto();
            return costo * 2;
        }

        public double CalcularMargen()
        {
            return CalcularPrecioVenta() - CalcularCosto();
        }
    }
}
