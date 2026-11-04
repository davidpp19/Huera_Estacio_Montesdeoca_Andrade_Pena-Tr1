using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_3.Models
{
    internal class Boleto
    {
        public int Id;
        public Viaje Viaje;
        public Pasajero Pasajero;
        public int Asiento;
        public decimal Precio;
        public DateTime FechaVenta;
        public bool Anulado;
        public DateTime FechaAnulacion;

        public Boleto(int id, Viaje viaje, Pasajero pasajero, int asiento, decimal precio)
        {
            Id = id;
            Viaje = viaje;
            Pasajero = pasajero;
            Asiento = asiento;
            Precio = precio;
            FechaVenta = DateTime.Now;
            Anulado = false;
        }

        public string Anular()
        {
            if (Anulado)
            {
                return "El boleto ya fue anulado.";
            }

            // Regla: solo hasta 2 horas antes de la salida
            DateTime limite = Viaje.Salida.AddHours(-2);
            if (DateTime.Now > limite)
            {
                return "Solo se puede anular hasta 2 horas antes de la salida.";
            }

            Anulado = true;
            FechaAnulacion = DateTime.Now;

            Viaje.Asientos[Asiento] = null;

            return "Boleto Anulado";
        }
    }
}
