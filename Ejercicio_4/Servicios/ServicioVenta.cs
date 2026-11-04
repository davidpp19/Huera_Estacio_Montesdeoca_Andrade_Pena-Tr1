using Ejercicio_4.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_4.Servicios
{
    public class ServicioVenta
    {
        public bool VenderPlato(Plato platoVendido, int cantidadVendida, List<RegistroVenta> listaVentas)
        {
            foreach (IngredienteReceta ingrediente in platoVendido.receta)
            {
                double cantidadNecesaria = ingrediente.cantidadUsada * cantidadVendida;
                if (ingrediente.ingredienteBase.stockActual < cantidadNecesaria)
                {
                    return false;
                }
            }

            foreach (IngredienteReceta ingrediente in platoVendido.receta)
            {
                double cantidadADescontar = ingrediente.cantidadUsada * cantidadVendida;
                ingrediente.ingredienteBase.stockActual = ingrediente.ingredienteBase.stockActual - cantidadADescontar;
            }

            RegistroVenta nuevaVenta = new RegistroVenta();
            nuevaVenta.nombreProducto = platoVendido.nombrePlato;
            nuevaVenta.cantidadVendida = cantidadVendida;
            nuevaVenta.totalCobrado = platoVendido.CalcularPrecioVenta() * cantidadVendida;
            nuevaVenta.fechaVenta = DateTime.Now;

            listaVentas.Add(nuevaVenta);

            return true;
        }
        public bool VenderBebida(Bebida bebidaVendida, int cantidadVendida, List<RegistroVenta> listaVentas)
        {
            if (bebidaVendida.stockActual >= cantidadVendida)
            {
                bebidaVendida.stockActual = bebidaVendida.stockActual - cantidadVendida;

                RegistroVenta nuevaVenta = new RegistroVenta();
                nuevaVenta.nombreProducto = bebidaVendida.nombreBebida;
                nuevaVenta.cantidadVendida = cantidadVendida;
                nuevaVenta.totalCobrado = bebidaVendida.CalcularPrecioVenta() * cantidadVendida;
                nuevaVenta.fechaVenta = DateTime.Now;

                listaVentas.Add(nuevaVenta);
                return true;
            }
            return false;
        }
    }
}
