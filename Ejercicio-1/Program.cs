
using Ejercicio_1.Modelos;
using Ejercicio_1.Persistencia;
using Ejercicio_1.Servicios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Ejercicio_1
{
    internal class Program
    {
        static Biblioteca biblioteca = new Biblioteca();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                GestorArchivos.cargar(biblioteca, buscarCarpetaDatos());
                Console.WriteLine("Datos cargados: " + biblioteca.materiales.Count + " materiales, "
                    + biblioteca.usuarios.Count + " usuarios, " + biblioteca.prestamos.Count + " préstamos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudieron cargar los datos: " + ex.Message);
            }

            bool salir = false;
            while (!salir)
            {
                mostrarMenu();
                String opcion = (Console.ReadLine() ?? "").Trim();
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case "1": opcionPrestar(); break;
                        case "2": opcionDevolver(); break;
                        case "3": opcionPagarMulta(); break;
                        case "4": opcionVerMaterialesYUsuarios(); break;
                        case "5": opcionVencidos(); break;
                        case "6": opcionHistorial(); break;
                        case "7": opcionMasPrestados(); break;
                        case "8": opcionTotalMultas(); break;
                        case "9": opcionAuditoria(); break;
                        case "0": salir = true; break;
                        default: Console.WriteLine("Opción no válida."); break;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
                catch (FormatException)
                {
                    Console.WriteLine("ERROR: dato mal escrito. Las fechas van así: 2026-11-03");
                }
            }
        }

        static void mostrarMenu()
        {
            Console.WriteLine();
            Console.WriteLine("========== BIBLIOTECA UTN ==========");
            Console.WriteLine(" 1. Prestar material");
            Console.WriteLine(" 2. Devolver material");
            Console.WriteLine(" 3. Pagar multa");
            Console.WriteLine(" 4. Ver materiales y usuarios");
            Console.WriteLine(" 5. Préstamos vencidos a la fecha");
            Console.WriteLine(" 6. Historial de un usuario");
            Console.WriteLine(" 7. Materiales más prestados");
            Console.WriteLine(" 8. Total de multas en un período");
            Console.WriteLine(" 9. Auditoría de ejemplares");
            Console.WriteLine(" 0. Salir");
            Console.Write("Opción: ");
        }


        static void opcionPrestar()
        {
            String idUsuario = leerTexto("Id del usuario (ej. U001): ");
            String codigo = leerTexto("Código del material (ej. L001): ");
            DateTime fecha = leerFecha("Fecha del préstamo");

            biblioteca.prestar(idUsuario, codigo, fecha);
            GestorArchivos.registrarPrestamo(idUsuario, codigo, fecha);      

            Prestamo p = biblioteca.prestamos[biblioteca.prestamos.Count - 1];
            Console.WriteLine("Préstamo " + p.id + " registrado. Debe devolverse hasta el "
                + formatoFecha(p.fechaVencimiento) + ".");
        }

        static void opcionDevolver()
        {
            String idPrestamo = leerTexto("Id del préstamo (ej. P8): ");
            DateTime fecha = leerFecha("Fecha de devolución");

            biblioteca.devolver(idPrestamo, fecha);
            GestorArchivos.registrarDevolucion(idPrestamo, fecha);           

            Prestamo p = biblioteca.buscarPrestamo(idPrestamo);
            if (p.multa > 0)
            {
                Console.WriteLine("Devuelto con retraso. Multa generada: " + formatoDinero(p.multa));
            }
            else
            {
                Console.WriteLine("Devuelto a tiempo. Sin multa.");
            }
        }

        static void opcionPagarMulta()
        {
            String idPrestamo = leerTexto("Id del préstamo (ej. P5): ");
            biblioteca.pagarMulta(idPrestamo);
            GestorArchivos.registrarPago(idPrestamo);                       
            Console.WriteLine("Multa pagada.");
        }

        static void opcionVerMaterialesYUsuarios()
        {
            Console.WriteLine("MATERIALES");
            Console.WriteLine(String.Format("{0,-6} {1,-8} {2,-38} {3,6} {4,6}", "Código", "Tipo", "Título", "Total", "Disp."));
            foreach (Material m in biblioteca.materiales)
            {
                Console.WriteLine(String.Format("{0,-6} {1,-8} {2,-38} {3,6} {4,6}",
                    m.codigo, tipoDe(m), m.titulo, m.ejemplaresTotales, m.ejemplaresDisponibles));
            }

            Console.WriteLine();
            Console.WriteLine("USUARIOS");
            foreach (Usuario u in biblioteca.usuarios)
            {
                String estado = u.tieneMultasPendientes() ? "con multa pendiente" : "al día";
                Console.WriteLine(u.id + "  " + u.nombre + "  (" + u.prestamosActivos() + " activos, " + estado + ")");
            }
        }


        static void opcionVencidos()
        {
            DateTime hoy = leerFecha("Fecha de corte");
            List<Prestamo> vencidos = biblioteca.prestamosVencidos(hoy);

            Console.WriteLine("PRÉSTAMOS VENCIDOS AL " + formatoFecha(hoy));
            if (vencidos.Count == 0)
            {
                Console.WriteLine("No hay préstamos vencidos.");
                return;
            }
            foreach (Prestamo p in vencidos)
            {
                int diasAtraso = (hoy - p.fechaVencimiento).Days;
                decimal multaEstimada = diasAtraso * p.material.multaDiaria;
                Console.WriteLine(p.id + "  " + p.usuario.nombre + "  " + p.material.titulo
                    + "  venció " + formatoFecha(p.fechaVencimiento)
                    + "  atraso " + diasAtraso + " días  multa estimada " + formatoDinero(multaEstimada));
            }
        }

        static void opcionHistorial()
        {
            String idUsuario = leerTexto("Id del usuario: ");
            Usuario u = biblioteca.buscarUsuario(idUsuario);
            if (u == null)
            {
                throw new InvalidOperationException("El usuario no existe");
            }

            Console.WriteLine("HISTORIAL DE " + u.nombre.ToUpper());
            List<Prestamo> historial = biblioteca.historialUsuario(idUsuario);
            if (historial.Count == 0)
            {
                Console.WriteLine("Sin préstamos.");
                return;
            }
            foreach (Prestamo p in historial)
            {
                imprimirPrestamo(p);
            }
        }

        static void opcionMasPrestados()
        {
            String texto = leerTexto("¿Cuántos mostrar? (Enter = 5): ");
            int cantidad = texto == "" ? 5 : int.Parse(texto);

            Console.WriteLine("MATERIALES MÁS PRESTADOS");
            int puesto = 1;
            foreach (Material m in biblioteca.materialesMasPrestados(cantidad))
            {
                Console.WriteLine(puesto + ". " + m.codigo + "  " + m.titulo + "  -> "
                    + biblioteca.contarPrestamos(m) + " préstamos");
                puesto++;
            }
        }

        static void opcionTotalMultas()
        {
            DateTime desde = leerFecha("Desde");
            DateTime hasta = leerFecha("Hasta");
            if (hasta < desde)
            {
                throw new InvalidOperationException("La fecha 'hasta' no puede ser anterior a 'desde'");
            }

            decimal total = biblioteca.totalMultas(desde, hasta);
            Console.WriteLine("Total de multas generadas del " + formatoFecha(desde) + " al "
                + formatoFecha(hasta) + ": " + formatoDinero(total));
        }

        static void opcionAuditoria()
        {
            Console.WriteLine("AUDITORÍA DE EJEMPLARES  (esperado = totales - prestados)");
            Console.WriteLine(String.Format("{0,-6} {1,6} {2,9} {3,6} {4,9}  {5}",
                "Código", "Total", "Prestados", "Disp.", "Esperado", "Estado"));

            int errores = 0;
            foreach (Material m in biblioteca.materiales)
            {
                int prestados = biblioteca.contarActivos(m);
                int esperado = m.ejemplaresTotales - prestados;
                bool ok = biblioteca.auditoriaCorrecta(m);
                if (!ok) errores++;

                Console.WriteLine(String.Format("{0,-6} {1,6} {2,9} {3,6} {4,9}  {5}",
                    m.codigo, m.ejemplaresTotales, prestados, m.ejemplaresDisponibles, esperado,
                    ok ? "OK" : "INCONSISTENTE"));
            }
            Console.WriteLine(errores == 0 ? "Todo cuadra." : "Hay " + errores + " materiales inconsistentes.");
        }



        static String leerTexto(String mensaje)
        {
            Console.Write(mensaje);
            return (Console.ReadLine() ?? "").Trim().ToUpper();
        }

        static DateTime leerFecha(String mensaje)
        {
            Console.Write(mensaje + " (aaaa-mm-dd, Enter = hoy): ");
            String texto = (Console.ReadLine() ?? "").Trim();
            if (texto == "")
            {
                return DateTime.Today;
            }
            return DateTime.Parse(texto, CultureInfo.InvariantCulture).Date;
        }

        static void imprimirPrestamo(Prestamo p)
        {
            String devolucion = p.fechaDevolucion.HasValue ? formatoFecha(p.fechaDevolucion.Value) : "-";
            String estado = p.estaDevuelto() ? "Devuelto" : "Activo";
            String multa = p.multa > 0 ? formatoDinero(p.multa) + (p.multaPagada ? " (pagada)" : " (PENDIENTE)") : "-";

            Console.WriteLine(p.id + "  " + p.material.titulo
                + "  prestado " + formatoFecha(p.fechaPrestamo)
                + "  vence " + formatoFecha(p.fechaVencimiento)
                + "  devuelto " + devolucion
                + "  " + estado + "  multa " + multa);
        }

        static String tipoDe(Material m)
        {
            return m is Libro ? "Libro" : "Revista";
        }

        static String formatoFecha(DateTime fecha)
        {
            return fecha.ToString("yyyy-MM-dd");
        }

        static String formatoDinero(decimal valor)
        {
            return "$" + valor.ToString("0.00");
        }

        static String buscarCarpetaDatos()
        {
            DirectoryInfo carpeta = new DirectoryInfo(AppContext.BaseDirectory);
            while (carpeta != null)
            {
                String posible = Path.Combine(carpeta.FullName, "datos");
                if (File.Exists(Path.Combine(posible, "materiales.json")))
                {
                    return posible;
                }
                carpeta = carpeta.Parent;
            }
            return "datos";
        }
    }
}