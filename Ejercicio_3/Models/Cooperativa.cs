using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_3.Models
{
    internal class Cooperativa
    {
        public List<Viaje> Viajes = new List<Viaje>();
        public List<Boleto> Boletos = new List<Boleto>();   

        public string[] Rutas = { "Ibarra-Quito", "Ibarra-Tulcán", "Ibarra-Otavalo" };
        public string[] TiposPasajero = { "Regular", "Tercera edad", "Discapacidad" };

        int siguienteIdViaje = 1;
        int siguienteIdBoleto = 1;

        // Crea un viaje y lo guarda en la lista
        public Viaje RegistrarViaje(string ruta, bool esDirecto, DateTime salida, int capacidad, decimal tarifa)
        {
            Viaje v = new Viaje(siguienteIdViaje, ruta, esDirecto, salida, capacidad, tarifa);
            siguienteIdViaje++;
            Viajes.Add(v);
            return v;
        }

        public Viaje BuscarViaje(int id)
        {
            foreach (Viaje v in Viajes)
            {
                if (v.Id == id) return v;
            }
            return null;
        }

        public Boleto BuscarBoleto(int id)
        {
            foreach (Boleto b in Boletos)
            {
                if (b.Id == id) return b;
            }
            return null;
        }

        public string VenderBoleto(Viaje viaje, Pasajero pasajero, int asiento)
        {
            string error = viaje.ValidarVenta(asiento);
            if (error != "")
            {
                return error;
            }

            decimal precio = viaje.CalcularPrecio(pasajero.Tipo);
            Boleto b = new Boleto(siguienteIdBoleto, viaje, pasajero, asiento, precio);
            siguienteIdBoleto++;

            viaje.Asientos[asiento] = b;   
            Boletos.Add(b);
            return "";
        }

        public string AnularBoleto(int idBoleto)
        {
            Boleto b = BuscarBoleto(idBoleto);
            if (b == null)
            {
                return "No existe ese boleto.";
            }
            return b.Anular();
        }
        public string EliminarViaje(int idViaje)
        {
            Viaje viaje = BuscarViaje(idViaje);
            if (viaje == null)
            {
                return "No existe ese viaje.";
            }

            // Regla: no se elimina si tiene boletos vendidos
            foreach (Boleto b in Boletos)
            {
                if (b.Viaje == viaje)
                {
                    return "El viaje tiene boletos vendidos, no se puede eliminar.";
                }
            }

            Viajes.Remove(viaje);
            return "Viaje Eliminado";
        }
    }
}
