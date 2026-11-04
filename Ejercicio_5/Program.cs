
using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text;
using Ejercicio_5.Models;
using Ejercicio_5.Servicios;

namespace Ejercicio_5
{
    internal class Program
    {
        private static TiendaServicio tienda;
        private static ReporteServicio reportes;
        private static string rutaDatos;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            tienda = new TiendaServicio();

            // Carpeta permanente para guardar los datos
            rutaDatos = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "Ejercicio_5",
                "tienda.json"
            );

            // Cargar datos al iniciar
            if (!InicializarDatos())
            {
                Console.WriteLine(
                    "\nPresiona Enter para cerrar.");
                Console.ReadLine();
                return;
            }

            reportes = new ReporteServicio(tienda);

            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                MostrarMenuPrincipal();

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        EjecutarOperacion(RegistrarArtesano);
                        break;

                    case "2":
                        EjecutarOperacion(RegistrarArtesania);
                        break;

                    case "3":
                        EjecutarOperacion(RecibirArtesanias);
                        break;

                    case "4":
                        EjecutarOperacion(VenderArtesanias);
                        break;

                    case "5":
                        EjecutarOperacion(DevolverArtesanias);
                        break;

                    case "6":
                        MostrarInventario();
                        Pausar();
                        break;

                    case "7":
                        EjecutarOperacion(EliminarArtesano);
                        break;

                    case "8":
                        EjecutarOperacion(EliminarArtesania);
                        break;

                    case "9":
                        MostrarMenuReportes();
                        break;

                    case "10":
                        MostrarCatalogo();
                        Pausar();
                        break;

                    case "0":
                        try
                        {
                            tienda.GuardarDatos(rutaDatos);
                            Console.WriteLine(
                                "\nDatos guardados correctamente.");
                            Console.WriteLine(
                                "Gracias por utilizar el sistema.");
                            salir = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                "Error al guardar: " + ex.Message);
                            Pausar();
                        }
                        break;

                    default:
                        Console.WriteLine(
                            "\nOpción no válida.");
                        Pausar();
                        break;
                }
            }
        }

        // ==========================================
        // CARGAR DATOS AL INICIAR
        // ==========================================
        static bool InicializarDatos()
        {
            try
            {
                // Primero buscar los datos permanentes
                if (File.Exists(rutaDatos))
                {
                    tienda.CargarDatos(rutaDatos);

                    Console.WriteLine(
                        "Datos anteriores recuperados.");
                    return true;
                }

                // Buscar el JSON incluido en el proyecto
                string carpetaPrograma =
                    AppDomain.CurrentDomain.BaseDirectory;

                string rutaEjemplo = Path.Combine(
                    carpetaPrograma,
                    "Datos",
                    "tienda.json"
                );

                // También admitir JSON en la raíz
                // si todavía no se movió a Datos
                if (!File.Exists(rutaEjemplo))
                {
                    rutaEjemplo = Path.Combine(
                        carpetaPrograma,
                        "tienda.json"
                    );
                }

                if (File.Exists(rutaEjemplo))
                {
                    tienda.CargarDatos(rutaEjemplo);

                    // Crear copia permanente para trabajar
                    tienda.GuardarDatos(rutaDatos);

                    Console.WriteLine(
                        "Datos iniciales importados.");
                }
                else
                {
                    Console.WriteLine(
                        "No hay datos previos. Tienda nueva.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "No se pudieron cargar los datos: " +
                    ex.Message);

                Console.WriteLine(
                    "Se detuvo el inicio para evitar " +
                    "sobrescribir la información existente.");

                return false;
            }
        }

        // ==========================================
        // MENÚ PRINCIPAL
        // ==========================================
        static void MostrarMenuPrincipal()
        {
            Console.WriteLine(
                "========================================");
            Console.WriteLine(
                "      TIENDA DE ARTESANÍAS OTAVALO");
            Console.WriteLine(
                "========================================");

            Console.WriteLine("\n         MENÚ PRINCIPAL\n");

            Console.WriteLine("1. Registrar artesano");
            Console.WriteLine("2. Registrar artesanía");
            Console.WriteLine("3. Recibir artesanías");
            Console.WriteLine("4. Vender artesanías");
            Console.WriteLine("5. Devolver artesanías");
            Console.WriteLine("6. Consultar inventario");
            Console.WriteLine("7. Eliminar artesano");
            Console.WriteLine("8. Eliminar artesanía");
            Console.WriteLine("9. Ver reportes");
            Console.WriteLine("10. Ver artesanos y artesanías");
            Console.WriteLine("0. Guardar y salir");

            Console.WriteLine(
                "\n========================================");

            Console.Write("Seleccione una opción: ");
        }

        // ==========================================
        // EJECUTAR Y GUARDAR OPERACIONES
        // ==========================================
        static void EjecutarOperacion(Action operacion)
        {
            Console.WriteLine();

            try
            {
                operacion();

                // Guardar después de la operación
                tienda.GuardarDatos(rutaDatos);

                Console.WriteLine(
                    "\nInformación guardada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "\nError: " + ex.Message);
            }

            Pausar();
        }

        // ==========================================
        // 1. REGISTRAR ARTESANO
        // ==========================================
        static void RegistrarArtesano()
        {
            Console.WriteLine(
                "=== REGISTRAR ARTESANO ===");

            int id = PedirEnteroPositivo(
                "ID del artesano: ");

            string nombre = PedirTexto(
                "Nombre del artesano: ");

            string comunidad = PedirTexto(
                "Comunidad: ");

            Artesano artesano = new Artesano
            {
                Id = id,
                Nombre = nombre,
                Comunidad = comunidad
            };

            tienda.RegistrarArtesano(artesano);

            Console.WriteLine(
                "\nArtesano registrado correctamente.");
        }

        // ==========================================
        // 2. REGISTRAR ARTESANÍA
        // ==========================================
        static void RegistrarArtesania()
        {
            Console.WriteLine(
                "=== REGISTRAR ARTESANÍA ===");

            MostrarArtesanos();

            int id = PedirEnteroPositivo(
                "\nID de la artesanía: ");

            string nombre = PedirTexto(
                "Nombre de la artesanía: ");

            Console.WriteLine("\nTipo de artesanía:");
            Console.WriteLine("1. Textil (20% comisión)");
            Console.WriteLine("2. Talla en madera (25% comisión)");

            int opcionTipo;

            do
            {
                opcionTipo = PedirEnteroPositivo(
                    "Seleccione el tipo: ");

                if (opcionTipo != 1 && opcionTipo != 2)
                {
                    Console.WriteLine(
                        "Seleccione 1 o 2.");
                }

            } while (opcionTipo != 1 && opcionTipo != 2);

            TipoArtesania tipo =
                opcionTipo == 1
                ? TipoArtesania.Textil
                : TipoArtesania.TallaMadera;

            decimal precio = PedirDecimalPositivo(
                "Precio de venta: $");

            int artesanoId = PedirEnteroPositivo(
                "ID del artesano propietario: ");

            Artesania artesania = new Artesania
            {
                Id = id,
                Nombre = nombre,
                Tipo = tipo,
                PrecioVenta = precio,
                ArtesanoId = artesanoId
            };

            tienda.RegistrarArtesania(artesania);

            Console.WriteLine(
                "\nArtesanía registrada correctamente.");
        }

        // ==========================================
        // 3. RECIBIR ARTESANÍAS
        // ==========================================
        static void RecibirArtesanias()
        {
            Console.WriteLine(
                "=== RECIBIR ARTESANÍAS ===");

            MostrarArtesanias();

            int idLote = PedirEnteroPositivo(
                "\nID del nuevo lote: ");

            int artesaniaId = PedirEnteroPositivo(
                "ID de la artesanía: ");

            int cantidad = PedirEnteroPositivo(
                "Cantidad recibida: ");

            DateTime fecha = PedirFecha(
                "Fecha (dd/MM/yyyy, Enter = hoy): ",
                true
            );

            tienda.RecibirArtesania(
                idLote,
                artesaniaId,
                cantidad,
                fecha
            );

            Console.WriteLine(
                "\nArtesanías recibidas correctamente.");

            Console.WriteLine(
                "Stock disponible: " +
                tienda.ObtenerStock(artesaniaId));
        }

        // ==========================================
        // 4. VENDER ARTESANÍAS
        // ==========================================
        static void VenderArtesanias()
        {
            Console.WriteLine(
                "=== VENDER ARTESANÍAS ===");

            MostrarInventario();

            int artesaniaId = PedirEnteroPositivo(
                "\nID de la artesanía: ");

            int cantidad = PedirEnteroPositivo(
                "Cantidad a vender: ");

            Artesania artesania =
                tienda.ObtenerArtesanias()
                .FirstOrDefault(
                    a => a.Id == artesaniaId);

            decimal total = tienda.VenderArtesania(
                artesaniaId,
                cantidad
            );

            Console.WriteLine(
                "\nVenta realizada correctamente.");

            Console.WriteLine(
                "Total vendido: $" + total.ToString("F2"));

            if (artesania != null)
            {
                decimal comision =
                    artesania.ObtenerComision() * cantidad;

                Console.WriteLine(
                    "Comisión de la tienda: $" +
                    comision.ToString("F2"));

                Console.WriteLine(
                    "Pago al artesano: $" +
                    (total - comision).ToString("F2"));
            }

            Console.WriteLine(
                "Stock restante: " +
                tienda.ObtenerStock(artesaniaId));
        }

        // ==========================================
        // 5. DEVOLVER ARTESANÍAS
        // ==========================================
        static void DevolverArtesanias()
        {
            Console.WriteLine(
                "=== DEVOLVER ARTESANÍAS ===");

            MostrarInventario();

            int artesaniaId = PedirEnteroPositivo(
                "\nID de la artesanía: ");

            int artesanoId = PedirEnteroPositivo(
                "ID del artesano propietario: ");

            int cantidad = PedirEnteroPositivo(
                "Cantidad a devolver: ");

            tienda.DevolverArtesania(
                artesaniaId,
                artesanoId,
                cantidad
            );

            Console.WriteLine(
                "\nDevolución realizada correctamente.");

            Console.WriteLine(
                "Stock restante: " +
                tienda.ObtenerStock(artesaniaId));
        }

        // ==========================================
        // 6. CONSULTAR INVENTARIO
        // ==========================================
        static void MostrarInventario()
        {
            Console.WriteLine(
                "\n=== INVENTARIO DISPONIBLE ===");

            var artesanias = tienda.ObtenerArtesanias();

            if (artesanias.Count == 0)
            {
                Console.WriteLine(
                    "No existen artesanías registradas.");
                return;
            }

            foreach (var artesania in artesanias)
            {
                Console.WriteLine(
                    "\nID: " + artesania.Id);

                Console.WriteLine(
                    "Producto: " + artesania.Nombre);

                Console.WriteLine(
                    "Tipo: " + artesania.Tipo);

                Console.WriteLine(
                    "Precio: $" +
                    artesania.PrecioVenta.ToString("F2"));

                Console.WriteLine(
                    "Stock: " +
                    tienda.ObtenerStock(artesania.Id));

                Console.WriteLine(
                    "----------------------------");
            }
        }

        // ==========================================
        // 7. ELIMINAR ARTESANO
        // ==========================================
        static void EliminarArtesano()
        {
            Console.WriteLine(
                "=== ELIMINAR ARTESANO ===");

            MostrarArtesanos();

            int id = PedirEnteroPositivo(
                "\nID del artesano a eliminar: ");

            if (!Confirmar())
            {
                Console.WriteLine(
                    "Operación cancelada.");
                return;
            }

            tienda.EliminarArtesano(id);

            Console.WriteLine(
                "\nArtesano eliminado correctamente.");
        }

        // ==========================================
        // 8. ELIMINAR ARTESANÍA
        // ==========================================
        static void EliminarArtesania()
        {
            Console.WriteLine(
                "=== ELIMINAR ARTESANÍA ===");

            MostrarArtesanias();

            int id = PedirEnteroPositivo(
                "\nID de la artesanía a eliminar: ");

            if (!Confirmar())
            {
                Console.WriteLine(
                    "Operación cancelada.");
                return;
            }

            tienda.EliminarArtesania(id);

            Console.WriteLine(
                "\nArtesanía eliminada correctamente.");
        }

        // ==========================================
        // 9. MENÚ DE REPORTES
        // ==========================================
        static void MostrarMenuReportes()
        {
            bool volver = false;

            while (!volver)
            {
                Console.Clear();

                Console.WriteLine(
                    "========================================");
                Console.WriteLine(
                    "          REPORTES DE LA TIENDA");
                Console.WriteLine(
                    "========================================");

                Console.WriteLine(
                    "\n1. Liquidación por artesano");
                Console.WriteLine(
                    "2. Artesanías pendientes de devolución");
                Console.WriteLine(
                    "3. Comunidades con más ventas");
                Console.WriteLine(
                    "4. Artesanías más vendidas");
                Console.WriteLine(
                    "5. Auditoría del inventario");
                Console.WriteLine(
                    "0. Volver al menú principal");

                Console.Write(
                    "\nSeleccione una opción: ");

                string opcion = Console.ReadLine();

                try
                {
                    DateTime inicio;
                    DateTime fin;

                    switch (opcion)
                    {
                        case "1":
                            Console.WriteLine(
                                "\n=== LIQUIDACIÓN ===");

                            MostrarArtesanos();

                            int artesanoId =
                                PedirEnteroPositivo(
                                "\nID del artesano: ");

                            PedirPeriodo(
                                out inicio,
                                out fin
                            );

                            reportes.MostrarLiquidacion(
                                artesanoId,
                                inicio,
                                fin
                            );
                            break;

                        case "2":
                            reportes.MostrarPendientesDevolucion();
                            break;

                        case "3":
                            PedirPeriodo(
                                out inicio,
                                out fin
                            );

                            reportes.MostrarComunidadesConMasVentas(
                                inicio,
                                fin
                            );
                            break;

                        case "4":
                            PedirPeriodo(
                                out inicio,
                                out fin
                            );

                            reportes.MostrarArtesaniasMasVendidas(
                                inicio,
                                fin
                            );
                            break;

                        case "5":
                            reportes.MostrarAuditoriaInventario();
                            break;

                        case "0":
                            volver = true;
                            break;

                        default:
                            Console.WriteLine(
                                "\nOpción no válida.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        "\nError: " + ex.Message);
                }

                if (!volver)
                {
                    Pausar();
                }
            }
        }

        // ==========================================
        // 10. CONSULTAR ARTESANOS Y ARTESANÍAS
        // ==========================================
        static void MostrarCatalogo()
        {
            MostrarArtesanos();
            MostrarArtesanias();
        }

        static void MostrarArtesanos()
        {
            Console.WriteLine(
                "\n=== ARTESANOS REGISTRADOS ===");

            var artesanos = tienda.ObtenerArtesanos();

            if (artesanos.Count == 0)
            {
                Console.WriteLine(
                    "No hay artesanos registrados.");
                return;
            }

            foreach (var artesano in artesanos)
            {
                Console.WriteLine(
                    "ID: " + artesano.Id +
                    " | Nombre: " + artesano.Nombre +
                    " | Comunidad: " + artesano.Comunidad);
            }
        }

        static void MostrarArtesanias()
        {
            Console.WriteLine(
                "\n=== ARTESANÍAS REGISTRADAS ===");

            var artesanias = tienda.ObtenerArtesanias();

            if (artesanias.Count == 0)
            {
                Console.WriteLine(
                    "No hay artesanías registradas.");
                return;
            }

            foreach (var artesania in artesanias)
            {
                Console.WriteLine(
                    "ID: " + artesania.Id +
                    " | Producto: " + artesania.Nombre +
                    " | Artesano ID: " + artesania.ArtesanoId +
                    " | Stock: " +
                    tienda.ObtenerStock(artesania.Id));
            }
        }

        // ==========================================
        // VALIDACIÓN DE DATOS INGRESADOS
        // ==========================================
        static int PedirEnteroPositivo(string mensaje)
        {
            int numero;

            while (true)
            {
                Console.Write(mensaje);

                string entrada = Console.ReadLine();

                if (int.TryParse(entrada, out numero)
                    && numero > 0)
                {
                    return numero;
                }

                Console.WriteLine(
                    "Ingrese un número entero mayor a cero.");
            }
        }

        static decimal PedirDecimalPositivo(string mensaje)
        {
            decimal numero;

            NumberStyles estilo =
                NumberStyles.AllowLeadingSign |
                NumberStyles.AllowDecimalPoint;

            while (true)
            {
                Console.Write(mensaje);

                string entrada = Console.ReadLine();

                bool valido = decimal.TryParse(
                    entrada,
                    estilo,
                    CultureInfo.GetCultureInfo("es-EC"),
                    out numero
                );

                if (!valido)
                {
                    valido = decimal.TryParse(
                        entrada,
                        estilo,
                        CultureInfo.InvariantCulture,
                        out numero
                    );
                }

                if (valido && numero > 0)
                {
                    return numero;
                }

                Console.WriteLine(
                    "Ingrese un precio válido mayor a cero.");
            }
        }

        static string PedirTexto(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);

                string texto = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return texto.Trim();
                }

                Console.WriteLine(
                    "Este campo no puede estar vacío.");
            }
        }

        static DateTime PedirFecha(
            string mensaje,
            bool permitirHoy)
        {
            while (true)
            {
                Console.Write(mensaje);

                string entrada = Console.ReadLine();

                if (permitirHoy &&
                    string.IsNullOrWhiteSpace(entrada))
                {
                    return DateTime.Today;
                }

                DateTime fecha;

                if (DateTime.TryParseExact(
                    entrada,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out fecha))
                {
                    return fecha;
                }

                Console.WriteLine(
                    "Fecha inválida. Use dd/MM/yyyy.");
            }
        }

        static void PedirPeriodo(
            out DateTime inicio,
            out DateTime fin)
        {
            Console.WriteLine(
                "\nIngrese el período del reporte.");

            while (true)
            {
                inicio = PedirFecha(
                    "Fecha inicial (dd/MM/yyyy): ",
                    false
                );

                fin = PedirFecha(
                    "Fecha final (dd/MM/yyyy): ",
                    false
                );

                if (inicio.Date <= fin.Date)
                {
                    return;
                }

                Console.WriteLine(
                    "La fecha inicial no puede ser " +
                    "mayor que la final.");
            }
        }

        static bool Confirmar()
        {
            Console.Write(
                "¿Confirma la eliminación? (S/N): ");

            string respuesta = Console.ReadLine();

            return string.Equals(
                respuesta,
                "S",
                StringComparison.OrdinalIgnoreCase
            );
        }

        static void Pausar()
        {
            Console.WriteLine(
                "\nPresiona Enter para continuar...");
            Console.ReadLine();
        }
    }
}
