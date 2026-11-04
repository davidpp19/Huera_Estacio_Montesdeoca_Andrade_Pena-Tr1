
using System;
using Ejercicio_5.Models;
using Ejercicio_5.Servicios;

namespace Ejercicio_5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crear el servicio de la tienda
            TiendaServicio tienda = new TiendaServicio();

            // 1. REGISTRAR ARTESANO
            Artesano artesano1 = new Artesano
            {
                Id = 1,
                Nombre = "Luis",
                Comunidad = "Peguche"
            };

            tienda.RegistrarArtesano(artesano1);

            // 2. REGISTRAR ARTESANIA
            Artesania artesania1 = new Artesania
            {
                Id = 1,
                Nombre = "Poncho artesanal",
                Tipo = TipoArtesania.Textil,
                PrecioVenta = 30.00m,
                ArtesanoId = artesano1.Id
            };

            tienda.RegistrarArtesania(artesania1);

            // 3. RECIBIR ARTESANIAS
            tienda.RecibirArtesania(
                1,
                artesania1.Id,
                10,
                DateTime.Today.AddDays(-95)
            );

            tienda.RecibirArtesania(
                2,
                artesania1.Id,
                5,
                DateTime.Today
            );

            // 4. MOSTRAR INFORMACION INICIAL
            Console.WriteLine("=== TIENDA OTAVALO ===");
            Console.WriteLine("Artesanos registrados: " +
                tienda.TotalArtesanos);
            Console.WriteLine("Artesanías registradas: " +
                tienda.TotalArtesanias);
            Console.WriteLine("Ponchos disponibles: " +
                tienda.ObtenerStock(1));
            Console.WriteLine("Movimientos registrados: " +
                tienda.TotalMovimientos);

            // 5. VENDER 3 PONCHOS
            decimal totalVenta = tienda.VenderArtesania(1, 3);

            decimal comision =
                artesania1.ObtenerComision() * 3;

            decimal pagoArtesano = totalVenta - comision;

            Console.WriteLine("\n=== VENTA DE ARTESANIAS ===");
            Console.WriteLine("Cantidad vendida: 3");
            Console.WriteLine("Total vendido: $" +
                totalVenta.ToString("F2"));
            Console.WriteLine("Comisión tienda: $" +
                comision.ToString("F2"));
            Console.WriteLine("Pago artesano: $" +
                pagoArtesano.ToString("F2"));
            Console.WriteLine("Stock restante: " +
                tienda.ObtenerStock(1));
            Console.WriteLine("Movimientos registrados: " +
                tienda.TotalMovimientos);

            // 6. VALIDAR VENTA SIN STOCK SUFICIENTE
            Console.WriteLine("\n=== VALIDACION DE STOCK ===");

            try
            {
                tienda.VenderArtesania(1, 20);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine("Stock después del intento: " +
                tienda.ObtenerStock(1));

            Console.WriteLine("Movimientos registrados: " +
                tienda.TotalMovimientos);

            // 7. DEVOLVER 2 PONCHOS
            Console.WriteLine("\n=== DEVOLUCION DE ARTESANIAS ===");

            tienda.DevolverArtesania(1, 1, 2);

            Console.WriteLine("Unidades devueltas: 2");
            Console.WriteLine("Stock restante: " +
                tienda.ObtenerStock(1));
            Console.WriteLine("Movimientos registrados: " +
                tienda.TotalMovimientos);

            // 8. VALIDAR PROPIETARIO
            Console.WriteLine("\n=== VALIDACION DE PROPIETARIO ===");

            try
            {
                tienda.DevolverArtesania(1, 2, 3);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine("Stock final: " +
                tienda.ObtenerStock(1));

            Console.WriteLine("Movimientos registrados: " +
                tienda.TotalMovimientos);

            // Mantener abierta la consola
            Console.WriteLine("\nPresiona una tecla para salir...");
            Console.ReadKey();
        }
    }
}
