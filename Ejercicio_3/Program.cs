using Ejercicio_3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_3
{
    internal class Program
    {
       
        static Cooperativa coop = new Cooperativa();

        static void Main(string[] args)
        {
            string opcion = "";

            while (opcion != "0")
            {
                Console.WriteLine();
                Console.WriteLine("===== COOPERATIVA DE TRANSPORTE - IBARRA =====");
                Console.WriteLine("1. Registrar viaje");
                Console.WriteLine("2. Listar viajes");
                Console.WriteLine("3. Vender boleto");
                Console.WriteLine("4. Anular boleto");
                Console.WriteLine("5. Eliminar viaje");
                Console.WriteLine("6. Reporte: mapa de asientos");
                Console.WriteLine("7. Reporte: recaudación por ruta");
                Console.WriteLine("8. Reporte: ocupación por viaje");
                Console.WriteLine("9. Reporte: boletos anulados");
                Console.WriteLine("10. Reporte: auditoría de asientos libres");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");
                opcion = Console.ReadLine();

                if (opcion == "1") RegistrarViaje();
                else if (opcion == "2") ListarViajes();
                else if (opcion == "3") VenderBoleto();
                else if (opcion == "4") AnularBoleto();
                else if (opcion == "5") EliminarViaje();
                else if (opcion == "6") MapaAsientos();
                else if (opcion == "7") Recaudacion();
                else if (opcion == "8") Reporte.Ocupacion(coop);
                else if (opcion == "9") Anulados();
                else if (opcion == "10") Reporte.AuditoriaAsientosLibres(coop);
                else if (opcion == "0") Console.WriteLine("Hasta luego.");
                else Console.WriteLine("Opción no válida.");
            }
        }

        static void RegistrarViaje()
        {
            Console.WriteLine("Rutas: 1) Ibarra-Quito  2) Ibarra-Tulcán  3) Ibarra-Otavalo");
            int numeroRuta = LeerEntero("Elige la ruta (1-3): ");
            if (numeroRuta < 1 || numeroRuta > 3)
            {
                Console.WriteLine("Ruta no válida.");
                return;
            }

            Console.Write("Tipo de viaje (1 = Directo, 2 = Con paradas): ");
            string tipo = Console.ReadLine();
            while (tipo != "1" && tipo != "2")
            {
                Console.Write("Valor no válido, escribe 1 o 2. Tipo de viaje (1 = Directo, 2 = Con paradas): ");
                tipo = Console.ReadLine();
            }
            bool esDirecto = (tipo == "1");

            DateTime salida = LeerFecha("Fecha y hora de salida (dd/MM/yyyy HH:mm): ");
            int capacidad = LeerEntero("Capacidad de asientos: ");
            decimal tarifa = LeerDecimal("Tarifa base: ");

            if (capacidad <= 0 || tarifa <= 0)
            {
                Console.WriteLine("La capacidad y la tarifa deben ser mayores que 0.");
                return;
            }

            Viaje v = coop.RegistrarViaje(coop.Rutas[numeroRuta - 1], esDirecto, salida, capacidad, tarifa);
            Console.WriteLine("Viaje " + v.Id + " registrado. Tarifa del viaje: $" + v.TarifaDelViaje());
        }

        static void ListarViajes()
        {
            Console.WriteLine();
            Console.WriteLine("--- Viajes registrados ---");

            foreach (Viaje v in coop.Viajes)
            {
                Console.WriteLine("Viaje " + v.Id + " | " + v.Ruta + " | " + v.Salida +
                    " | tarifa $" + v.TarifaDelViaje() +
                    " | libres " + v.ContarLibres() + "/" + v.Capacidad);
            }
        }

        static void VenderBoleto()
        {
            ListarViajes();
            int idViaje = LeerEntero("Id del viaje: ");

            Viaje viaje = coop.BuscarViaje(idViaje);
            if (viaje == null)
            {
                Console.WriteLine("No existe ese viaje.");
                return;
            }

            Console.Write("Nombre del pasajero: ");
            string nombre = Console.ReadLine();

            Console.Write("Cédula: ");
            string cedula = Console.ReadLine();

            Console.WriteLine("Tipo: 1) Regular  2) Tercera edad  3) Discapacidad");
            int tipo = LeerEntero("Elige el tipo (1-3): ");
            if (tipo < 1 || tipo > 3)
            {
                Console.WriteLine("Tipo no válido.");
                return;
            }

            int asiento = LeerEntero("Número de asiento: ");

            Pasajero pasajero = new Pasajero(nombre, cedula, coop.TiposPasajero[tipo - 1]);
            string error = coop.VenderBoleto(viaje, pasajero, asiento);

            if (error != "")
            {
                Console.WriteLine("ERROR: " + error);
            }
            else
            {
                Console.WriteLine("Boleto vendido. Precio: $" + viaje.CalcularPrecio(pasajero.Tipo));
            }
        }

        static void AnularBoleto()
        {
            int id = LeerEntero("Id del boleto a anular: ");

            string error = coop.AnularBoleto(id);
            if (error != "")
            {
                Console.WriteLine("ERROR: " + error);
            }
            else
            {
                Console.WriteLine("Boleto anulado. El asiento quedó disponible.");
            }
        }

        static void EliminarViaje()
        {
            int id = LeerEntero("Id del viaje a eliminar: ");

            string error = coop.EliminarViaje(id);
            if (error != "")
            {
                Console.WriteLine("ERROR: " + error);
            }
            else
            {
                Console.WriteLine("Viaje eliminado.");
            }
        }

        static void MapaAsientos()
        {
            int id = LeerEntero("Id del viaje: ");

            Viaje v = coop.BuscarViaje(id);
            if (v == null)
            {
                Console.WriteLine("No existe ese viaje.");
            }
            else
            {
                Reporte.MapaAsientos(v);
            }
        }

        static void Recaudacion()
        {
            DateTime desde = LeerFecha("Desde (dd/MM/yyyy): ");
            DateTime hasta = LeerFecha("Hasta (dd/MM/yyyy): ");
            Reporte.Recaudacion(coop, desde, hasta);
        }

        static void Anulados()
        {
            DateTime desde = LeerFecha("Desde (dd/MM/yyyy): ");
            DateTime hasta = LeerFecha("Hasta (dd/MM/yyyy): ");
            Reporte.Anulados(coop, desde, hasta);
        }

        // ===== Métodos para leer datos sin que el programa se cierre =====
        // Cada uno repite la pregunta mientras el dato no tenga el formato correcto

        static int LeerEntero(string mensaje)
        {
            int numero;
            Console.Write(mensaje);
            while (int.TryParse(Console.ReadLine(), out numero) == false)
            {
                Console.Write("Valor no válido, escribe un número entero. " + mensaje);
            }
            return numero;
        }

        static decimal LeerDecimal(string mensaje)
        {
            decimal numero;
            Console.Write(mensaje);
            while (decimal.TryParse(Console.ReadLine(), out numero) == false)
            {
                Console.Write("Valor no válido, escribe un número. " + mensaje);
            }
            return numero;
        }

        static DateTime LeerFecha(string mensaje)
        {
            DateTime fecha;
            Console.Write(mensaje);
            while (DateTime.TryParse(Console.ReadLine(), out fecha) == false)
            {
                Console.Write("Fecha no válida. " + mensaje);
            }
            return fecha;
        }
    }
}
