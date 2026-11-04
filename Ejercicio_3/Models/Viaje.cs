using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_3.Models
{
    internal class Viaje
    {
        public int Id;
        public string Ruta;          // "Ibarra-Quito", "Ibarra-Tulcán" o "Ibarra-Otavalo"
        public bool EsDirecto;       // true = directo, false = con paradas
        public DateTime Salida;      
        public int Capacidad;        
        public decimal TarifaBase;

        // Mapa de asientos: la posición 5 guarda el boleto del asiento 5.
        // Si la posición es null, el asiento está libre.
        public Boleto[] Asientos;

        public Viaje(int id, string ruta, bool esDirecto, DateTime salida, int capacidad, decimal tarifaBase)
        {
            Id = id;
            Ruta = ruta;
            EsDirecto = esDirecto;
            Salida = salida;
            Capacidad = capacidad;
            TarifaBase = tarifaBase;
            Asientos = new Boleto[capacidad + 1];
        }

        // Directo cobra el 100 %, con paradas cobra el 85 %
        public decimal TarifaDelViaje()
        {
            if (EsDirecto)
            {
                return TarifaBase;
            }
            return Math.Round(TarifaBase * 0.85m, 2);
        }

        // Tercera edad y discapacidad pagan el 50 % de la tarifa del viaje
        public decimal CalcularPrecio(string tipoPasajero)
        {
            decimal tarifa = TarifaDelViaje();
            if (tipoPasajero == "Regular")
            {
                return tarifa;
            }
            return Math.Round(tarifa * 0.50m, 2);
        }

        // Cuenta los asientos que tienen un boleto
        public int ContarOcupados()
        {
            int contador = 0;
            for (int i = 1; i <= Capacidad; i++)
            {
                if (Asientos[i] != null)
                {
                    contador++;
                }
            }
            return contador;
        }

        public int ContarLibres()
        {
            return Capacidad - ContarOcupados();
        }

        public double PorcentajeOcupacion()
        {
            return ContarOcupados() * 100.0 / Capacidad;
        }

        // true si la hora de salida ya pasó
        public bool YaSalio()
        {
            return Salida <= DateTime.Now;
        }

        // Revisa si se puede vender ese asiento. Devuelve "" si se puede, o el error.
        public string ValidarVenta(int asiento)
        {
            if (YaSalio())
            {
                return "El viaje ya salió, no se pueden vender boletos.";
            }
            if (asiento < 1 || asiento > Capacidad)
            {
                return "El asiento debe estar entre 1 y " + Capacidad + ".";
            }
            if (Asientos[asiento] != null)
            {
                return "El asiento " + asiento + " ya está ocupado.";
            }
            return "";
        }
    }
}
