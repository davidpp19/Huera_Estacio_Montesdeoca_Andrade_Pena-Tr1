
using System;
using System.IO;
using Ejercicio_5.Models;
using Ejercicio_5.Servicios;

namespace Ejercicio_5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crear el servicio principal
            TiendaServicio tienda = new TiendaServicio();

            // ==========================================
            // 1. REGISTRAR ARTESANOS
            // ==========================================
            Artesano artesano1 = new Artesano
            {
                Id = 1,
                Nombre = "Luis",
                Comunidad = "Peguche"
            };

            tienda.RegistrarArtesano(artesano1);

            // ==========================================
            // 2. REGISTRAR ARTESANIAS
            // ==========================================
            Artesania artesania1 = new Artesania
            {
                Id = 1,
                Nombre = "Poncho artesanal",
                Tipo = TipoArtesania.Textil,
                PrecioVenta = 30.00m,
                ArtesanoId = artesano1.Id
            };

            tienda.RegistrarArtesania(artesania1);

            // ==========================================
            // 3. RECIBIR ARTESANIAS
            // ==========================================
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

            // ==========================================
            // 4. MOSTRAR INFORMACION INICIAL
            // ==========================================
            Console.WriteLine("=== TIENDA OTAVALO ===");

            Console.WriteLine("Artesanos registrados: " +
                tienda.TotalArtesanos);

            Console.WriteLine("Artesanías registradas: " +
                tienda.TotalArtesanias);

            Console.WriteLine("Ponchos disponibles: " +
                tienda.ObtenerStock(1));

            Console.WriteLine("Movimientos registrados: " +
                tienda.TotalMovimientos);

            // ==========================================
            // 5. VENDER 3 PONCHOS
            // ==========================================
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

            // ==========================================
            // 6. VALIDAR STOCK INSUFICIENTE
            // ==========================================
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

            // ==========================================
            // 7. DEVOLVER 2 PONCHOS
            // ==========================================
            Console.WriteLine("\n=== DEVOLUCION DE ARTESANIAS ===");

            tienda.DevolverArtesania(1, 1, 2);

            Console.WriteLine("Unidades devueltas: 2");

            Console.WriteLine("Stock restante: " +
                tienda.ObtenerStock(1));

            Console.WriteLine("Movimientos registrados: " +
                tienda.TotalMovimientos);

            // ==========================================
            // 8. VALIDAR PROPIETARIO
            // ==========================================
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

            // ==========================================
            // 9. VALIDAR ELIMINACION DE ARTESANO
            // ==========================================
            Console.WriteLine("\n=== ELIMINAR ARTESANO ===");

            try
            {
                tienda.EliminarArtesano(1);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine("Artesanos registrados: " +
                tienda.TotalArtesanos);

            // ==========================================
            // 10. VALIDAR ELIMINACION DE ARTESANIA
            // ==========================================
            Console.WriteLine("\n=== ELIMINAR ARTESANIA ===");

            try
            {
                tienda.EliminarArtesania(1);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine("Artesanías registradas: " +
                tienda.TotalArtesanias);

            // ==========================================
            // 11. PROBAR ELIMINACIONES PERMITIDAS
            // ==========================================
            Console.WriteLine("\n=== ELIMINACION PERMITIDA ===");

            // Artesano sin artesanías
            Artesano artesano2 = new Artesano
            {
                Id = 2,
                Nombre = "Carlos",
                Comunidad = "Agato"
            };

            tienda.RegistrarArtesano(artesano2);
            tienda.EliminarArtesano(2);

            Console.WriteLine("Artesano sin productos eliminado.");

            Console.WriteLine("Artesanos restantes: " +
                tienda.TotalArtesanos);

            // Artesanía sin movimientos
            Artesania artesania2 = new Artesania
            {
                Id = 2,
                Nombre = "Bufanda artesanal",
                Tipo = TipoArtesania.Textil,
                PrecioVenta = 15.00m,
                ArtesanoId = 1
            };

            tienda.RegistrarArtesania(artesania2);
            tienda.EliminarArtesania(2);

            Console.WriteLine("Artesanía sin movimientos eliminada.");

            Console.WriteLine("Artesanías restantes: " +
                tienda.TotalArtesanias);

            // ==========================================
            // 12. REGISTRAR OTRO ARTESANO
            // PARA PROBAR LOS REPORTES
            // ==========================================
            Console.WriteLine("\n=== REGISTRO DE ARTESANO DE AGATO ===");

            Artesano artesano3 = new Artesano
            {
                Id = 3,
                Nombre = "Maria",
                Comunidad = "Agato"
            };

            tienda.RegistrarArtesano(artesano3);

            Artesania artesania3 = new Artesania
            {
                Id = 3,
                Nombre = "Figura de madera",
                Tipo = TipoArtesania.TallaMadera,
                PrecioVenta = 40.00m,
                ArtesanoId = artesano3.Id
            };

            tienda.RegistrarArtesania(artesania3);

            // Recibir 4 figuras de madera
            tienda.RecibirArtesania(
                3,
                artesania3.Id,
                4,
                DateTime.Today
            );

            // Vender 2 figuras
            decimal ventaMadera =
                tienda.VenderArtesania(3, 2);

            decimal comisionMadera =
                artesania3.ObtenerComision() * 2;

            decimal pagoMadera =
                ventaMadera - comisionMadera;

            Console.WriteLine("Artesano: " + artesano3.Nombre);
            Console.WriteLine("Comunidad: " + artesano3.Comunidad);
            Console.WriteLine("Producto: " + artesania3.Nombre);

            Console.WriteLine("Total vendido: $" +
                ventaMadera.ToString("F2"));

            Console.WriteLine("Comisión tienda (25%): $" +
                comisionMadera.ToString("F2"));

            Console.WriteLine("Pago artesano: $" +
                pagoMadera.ToString("F2"));

            Console.WriteLine("Stock restante: " +
                tienda.ObtenerStock(3));

            // ==========================================
            // 13. GUARDAR DATOS EN JSON
            // ==========================================
            Console.WriteLine("\n=== GUARDAR INFORMACION ===");

            string ruta = Path.Combine(
                AppContext.BaseDirectory,
                "Datos",
                "tienda.json"
            );

            tienda.GuardarDatos(ruta);

            Console.WriteLine("Datos guardados correctamente.");
            Console.WriteLine("Ubicación: " + ruta);

            // ==========================================
            // 14. RECUPERAR DATOS DESDE JSON
            // ==========================================
            Console.WriteLine("\n=== RECUPERAR INFORMACION ===");

            TiendaServicio tiendaRecuperada =
                new TiendaServicio();

            tiendaRecuperada.CargarDatos(ruta);

            Console.WriteLine("Artesanos recuperados: " +
                tiendaRecuperada.TotalArtesanos);

            Console.WriteLine("Artesanías recuperadas: " +
                tiendaRecuperada.TotalArtesanias);

            Console.WriteLine("Stock de ponchos: " +
                tiendaRecuperada.ObtenerStock(1));

            Console.WriteLine("Stock de figuras: " +
                tiendaRecuperada.ObtenerStock(3));

            Console.WriteLine("Movimientos recuperados: " +
                tiendaRecuperada.TotalMovimientos);

            // ==========================================
            // 15. CREAR SERVICIO DE REPORTES
            // ==========================================
            ReporteServicio reportes =
                new ReporteServicio(tiendaRecuperada);

            DateTime fechaInicio =
                DateTime.Today.AddDays(-30);

            DateTime fechaFin =
                DateTime.Today;

            // ==========================================
            // 16. REPORTE 1: LIQUIDACION
            // ==========================================
            reportes.MostrarLiquidacion(
                1,
                fechaInicio,
                fechaFin
            );

            // Liquidación de María
            reportes.MostrarLiquidacion(
                3,
                fechaInicio,
                fechaFin
            );

            // ==========================================
            // 17. REPORTE 2: DEVOLUCIONES PENDIENTES
            // ==========================================
            reportes.MostrarPendientesDevolucion();

            // ==========================================
            // 18. REPORTE 3: COMUNIDADES CON MAS VENTAS
            // ==========================================
            reportes.MostrarComunidadesConMasVentas(
                fechaInicio,
                fechaFin
            );

            // ==========================================
            // 19. REPORTE 4: ARTESANIAS MAS VENDIDAS
            // ==========================================
            reportes.MostrarArtesaniasMasVendidas(
                fechaInicio,
                fechaFin
            );

            // ==========================================
            // 20. REPORTE 5: AUDITORIA DE INVENTARIO
            // ==========================================
            reportes.MostrarAuditoriaInventario();

            // ==========================================
            // 21. FINALIZAR PROGRAMA
            // ==========================================
            Console.WriteLine("\n=== PRUEBAS FINALIZADAS ===");
            Console.WriteLine("Presiona una tecla para salir...");

            Console.ReadKey();
        }
    }
}
