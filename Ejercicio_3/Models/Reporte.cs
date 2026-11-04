using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_3.Models
{
    internal class Reporte
    {
        // 1) Mapa de asientos: muestra cada asiento como ocupado o libre
        public static void MapaAsientos(Viaje v)
        {
            Console.WriteLine();
            Console.WriteLine("Mapa de asientos del viaje " + v.Id + " (" + v.Ruta + ")");

            for (int i = 1; i <= v.Capacidad; i++)
            {
                if (v.Asientos[i] != null)
                {
                    Console.WriteLine("Asiento " + i + ": OCUPADO por " + v.Asientos[i].Pasajero.Nombre);
                }
                else
                {
                    Console.WriteLine("Asiento " + i + ": libre");
                }
            }

            Console.WriteLine("Ocupados: " + v.ContarOcupados());
            Console.WriteLine("Libres: " + v.ContarLibres());
        }

        // 2) Recaudación por ruta en un período
        public static void Recaudacion(Cooperativa coop, DateTime desde, DateTime hasta)
        {
            Console.WriteLine();
            Console.WriteLine("Recaudación por ruta");

            foreach (string ruta in coop.Rutas)
            {
                decimal total = 0;

                foreach (Boleto b in coop.Boletos)
                {
                    if (b.Anulado == false && b.Viaje.Ruta == ruta &&
                        b.FechaVenta.Date >= desde.Date && b.FechaVenta.Date <= hasta.Date)
                    {
                        total = total + b.Precio;
                    }
                }

                Console.WriteLine(ruta + ": $" + total);
            }
        }

        // 3) Porcentaje de ocupación de cada viaje
        public static void Ocupacion(Cooperativa coop)
        {
            Console.WriteLine();
            Console.WriteLine("Ocupación de cada viaje");

            foreach (Viaje v in coop.Viajes)
            {
                Console.WriteLine("Viaje " + v.Id + " (" + v.Ruta + "): " + v.PorcentajeOcupacion() + " %");
            }
        }

        // 4) Boletos anulados en un período
        public static void Anulados(Cooperativa coop, DateTime desde, DateTime hasta)
        {
            Console.WriteLine();
            Console.WriteLine("Boletos anulados");

            foreach (Boleto b in coop.Boletos)
            {
                if (b.Anulado == true &&
                    b.FechaAnulacion.Date >= desde.Date && b.FechaAnulacion.Date <= hasta.Date)
                {
                    Console.WriteLine("Boleto " + b.Id + " | Viaje " + b.Viaje.Id +
                        " | Asiento " + b.Asiento + " | " + b.Pasajero.Nombre);
                }
            }
        }

        // 5) Auditoría de asientos libres
        public static void AuditoriaAsientosLibres(Cooperativa coop)
        {
            Console.WriteLine();
            Console.WriteLine("Auditoría de asientos libres");

            foreach (Viaje v in coop.Viajes)
            {
                int libres = 0;
                for (int i = 1; i <= v.Capacidad; i++)
                {
                    if (v.Asientos[i] == null)
                    {
                        libres++;
                    }
                }

                int boletosActivos = 0;
                foreach (Boleto b in coop.Boletos)
                {
                    if (b.Viaje == v && b.Anulado == false)
                    {
                        boletosActivos++;
                    }
                }

                Console.WriteLine("Viaje " + v.Id + ": capacidad " + v.Capacidad +
                    ", libres " + libres + ", boletos activos " + boletosActivos);

                if (libres + boletosActivos == v.Capacidad)
                {
                    Console.WriteLine("   Resultado: correcto");
                }
                else
                {
                    Console.WriteLine("   Resultado: HAY UN ERROR, los números no cuadran");
                }
            }
        }
    }
}
