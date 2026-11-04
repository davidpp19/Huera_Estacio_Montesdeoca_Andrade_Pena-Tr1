using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Ejercicio_3.Models;

namespace Ejercicio_3
{

    // Estas dos clases solo sirven para leer el JSON.
    // Usan { get; set; } porque el lector de JSON necesita propiedades.
    class ViajeDatos
    {
        public int Id { get; set; }
        public string Ruta { get; set; }
        public bool EsDirecto { get; set; }
        public DateTime Salida { get; set; }
        public int Capacidad { get; set; }
        public decimal TarifaBase { get; set; }
    }

    class BoletoDatos
    {
        public int IdViaje { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string TipoPasajero { get; set; }
        public int Asiento { get; set; }
        public bool Anulado { get; set; }
    }

    // Lee los archivos JSON y llena la cooperativa
    class CargadorDatos
    {
        public static void Cargar(Cooperativa coop)
        {
            // Los JSON se copian a la misma carpeta donde está el programa
            string carpeta = AppContext.BaseDirectory;
            string rutaViajes = Path.Combine(carpeta, "viajes.json");
            string rutaBoletos = Path.Combine(carpeta, "boletos.json");

            if (File.Exists(rutaViajes) == false || File.Exists(rutaBoletos) == false)
            {
                Console.WriteLine("No se encontraron viajes.json y boletos.json. El programa inicia vacío.");
                return;
            }

            try
            {
                // 1) Leer los viajes
                string textoViajes = File.ReadAllText(rutaViajes);
                List<ViajeDatos> listaViajes = JsonSerializer.Deserialize<List<ViajeDatos>>(textoViajes);

                foreach (ViajeDatos d in listaViajes)
                {
                    coop.RegistrarViaje(d.Ruta, d.EsDirecto, d.Salida, d.Capacidad, d.TarifaBase);
                }

                // 2) Leer los boletos
                string textoBoletos = File.ReadAllText(rutaBoletos);
                List<BoletoDatos> listaBoletos = JsonSerializer.Deserialize<List<BoletoDatos>>(textoBoletos);

                foreach (BoletoDatos d in listaBoletos)
                {
                    Viaje viaje = coop.BuscarViaje(d.IdViaje);
                    if (viaje == null)
                    {
                        Console.WriteLine("Boleto ignorado: no existe el viaje " + d.IdViaje);
                        continue;
                    }

                    Pasajero pasajero = new Pasajero(d.Nombre, d.Cedula, d.TipoPasajero);
                    string error = coop.VenderBoleto(viaje, pasajero, d.Asiento);

                    if (error != "")
                    {
                        Console.WriteLine("Boleto ignorado (" + d.Nombre + "): " + error);
                        continue;
                    }

                    // Si el JSON dice que está anulado, se anula el boleto recién creado
                    if (d.Anulado == true)
                    {
                        Boleto ultimo = coop.Boletos[coop.Boletos.Count - 1];
                        ultimo.Anular();
                    }
                }

                Console.WriteLine("Datos cargados: " + coop.Viajes.Count + " viajes y " +
                    coop.Boletos.Count + " boletos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer los archivos JSON: " + ex.Message);
            }
        }
    }
}
