using Ejercicio_4.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_4.Servicios
{
    public class ServicioInventario
    {
        public void AbastecerIngrediente(Ingrediente ingrediente, double cantidad)
        {
            ingrediente.stockActual = ingrediente.stockActual + cantidad;
        }

        public void AbastecerBebida(Bebida bebida, int cantidad)
        {
            bebida.stockActual = bebida.stockActual + cantidad;
        }
        public List<Ingrediente> ObtenerListaCompras(List<Ingrediente> listaCompleta)
        {
            List<Ingrediente> listaCompras = new List<Ingrediente>();
            foreach (Ingrediente ingrediente in listaCompleta)
            {
                if (ingrediente.stockActual < ingrediente.stockMinimo)
                {
                    listaCompras.Add(ingrediente);
                }
            }
            return listaCompras;
        }
        public int CalcularPlatosPosibles(Plato plato)
        {
            if (plato.receta.Count == 0)
            {
                return 0;
            }

            int cantidadMinima = 999999; 

            foreach (IngredienteReceta ingrediente in plato.receta)
            {
                int cantidadPosible = (int)(ingrediente.ingredienteBase.stockActual / ingrediente.cantidadUsada);

                if (cantidadPosible < cantidadMinima)
                {
                    cantidadMinima = cantidadPosible;
                }
            }

            return cantidadMinima;
        }
        public bool PuedeEliminarIngrediente(Ingrediente ingrediente)
        {
            if (ingrediente.formaParteDeReceta == true)
            {
                return false;
            }
            return true;
        }
    }
}
