using CooperativaImbabura.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_2
{

    public class Program
    {
        private static Cooperativa cooperativaImbabura = new Cooperativa("datos");

        public static void Main(string[] args)
        {
            int opcionElegida = -1;

            while (opcionElegida != 0)
            {
                MostrarMenu();
                opcionElegida = LeerNumeroEntero("Elija una opción: ");
                Console.WriteLine();

                try
                {
                    switch (opcionElegida)
                    {
                        case 1: RegistrarSocio(); break;
                        case 2: AbrirCuenta(); break;
                        case 3: VerCuentasDeUnSocio(); break;
                        case 4: Depositar(); break;
                        case 5: Retirar(); break;
                        case 6: Transferir(); break;
                        case 7: AplicarIntereses(); break;
                        case 8: CerrarCuenta(); break;
                        case 9: EliminarCuenta(); break;
                        case 10: VerEstadoDeCuenta(); break;
                        case 11: VerInteresesDelMes(); break;
                        case 12: VerCuentasEnSobregiro(); break;
                        case 13: VerTotalesDelPeriodo(); break;
                        case 14: VerAuditoriaDeSaldos(); break;
                        case 0: Console.WriteLine("Gracias por usar el sistema."); break;
                        default: Console.WriteLine("Opción no válida."); break;
                    }
                }
                catch (Exception errorDeOperacion)
                {
                    Console.WriteLine("ERROR: " + errorDeOperacion.Message);
                }

                if (opcionElegida != 0)
                {
                    Console.WriteLine("\nPresione ENTER para continuar...");
                    Console.ReadLine();
                }
            }
        }

        private static void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("===== COOPERATIVA DE AHORRO Y CRÉDITO IMBABURA =====");
            Console.WriteLine(" 1. Registrar socio");
            Console.WriteLine(" 2. Abrir cuenta");
            Console.WriteLine(" 3. Ver cuentas de un socio");
            Console.WriteLine(" 4. Depositar");
            Console.WriteLine(" 5. Retirar");
            Console.WriteLine(" 6. Transferir");
            Console.WriteLine(" 7. Aplicar intereses del mes (cuentas de ahorro)");
            Console.WriteLine(" 8. Cerrar cuenta");
            Console.WriteLine(" 9. Eliminar cuenta");
            Console.WriteLine("----------------- REPORTES -----------------");
            Console.WriteLine("10. Estado de cuenta");
            Console.WriteLine("11. Intereses del mes");
            Console.WriteLine("12. Cuentas en sobregiro");
            Console.WriteLine("13. Total depositado y retirado en un período");
            Console.WriteLine("14. Auditoría de saldos");
            Console.WriteLine(" 0. Salir");
        }


        private static void RegistrarSocio()
        {
            string cedulaSocio = LeerTexto("Cédula: ");
            string nombreCompletoSocio = LeerTexto("Nombre completo: ");
            cooperativaImbabura.RegistrarSocio(cedulaSocio, nombreCompletoSocio);
            Console.WriteLine("Socio registrado correctamente.");
        }

        private static void AbrirCuenta()
        {
            string cedulaSocio = LeerTexto("Cédula del socio: ");
            Console.WriteLine("1. Cuenta de ahorros");
            Console.WriteLine("2. Cuenta corriente");
            int tipoDeCuentaElegido = LeerNumeroEntero("Tipo de cuenta: ");

            Cuenta cuentaAbierta;
            if (tipoDeCuentaElegido == 1)
            {
                cuentaAbierta = cooperativaImbabura.AbrirCuentaAhorros(cedulaSocio);
            }
            else if (tipoDeCuentaElegido == 2)
            {
                cuentaAbierta = cooperativaImbabura.AbrirCuentaCorriente(cedulaSocio);
            }
            else
            {
                Console.WriteLine("Tipo de cuenta no válido.");
                return;
            }
            Console.WriteLine("Cuenta abierta. Número de cuenta: " + cuentaAbierta.NumeroCuenta);
        }

        private static void VerCuentasDeUnSocio()
        {
            string cedulaSocio = LeerTexto("Cédula del socio: ");
            Socio socioBuscado = cooperativaImbabura.BuscarSocio(cedulaSocio);
            if (socioBuscado == null)
            {
                Console.WriteLine("No existe un socio con esa cédula.");
                return;
            }

            Console.WriteLine("Cuentas de " + socioBuscado.NombreCompletoSocio + ":");
            if (socioBuscado.CuentasDelSocio.Count == 0)
            {
                Console.WriteLine("  (no tiene cuentas)");
            }
            foreach (Cuenta cuentaDelSocio in socioBuscado.CuentasDelSocio)
            {
                Console.WriteLine("  " + cuentaDelSocio.NumeroCuenta
                                  + " | " + cuentaDelSocio.ObtenerTipoCuenta()
                                  + " | Saldo: " + cuentaDelSocio.SaldoActual.ToString("0.00")
                                  + " | " + cuentaDelSocio.EstadoCuenta);
            }
        }

        private static void Depositar()
        {
            string numeroCuenta = LeerTexto("Número de cuenta: ");
            decimal montoADepositar = LeerMonto("Monto a depositar: ");
            cooperativaImbabura.Depositar(numeroCuenta, montoADepositar);
            Console.WriteLine("Depósito realizado.");
        }

        private static void Retirar()
        {
            string numeroCuenta = LeerTexto("Número de cuenta: ");
            decimal montoARetirar = LeerMonto("Monto a retirar: ");
            cooperativaImbabura.Retirar(numeroCuenta, montoARetirar);
            Console.WriteLine("Retiro realizado.");
        }

        private static void Transferir()
        {
            string numeroCuentaOrigen = LeerTexto("Cuenta de origen: ");
            string numeroCuentaDestino = LeerTexto("Cuenta de destino: ");
            decimal montoATransferir = LeerMonto("Monto a transferir: ");
            cooperativaImbabura.Transferir(numeroCuentaOrigen, numeroCuentaDestino, montoATransferir);
            Console.WriteLine("Transferencia realizada.");
        }

        private static void AplicarIntereses()
        {
            cooperativaImbabura.AplicarInteresesDelMes();
            Console.WriteLine("Intereses del mes aplicados a las cuentas de ahorro.");
        }

        private static void CerrarCuenta()
        {
            string numeroCuenta = LeerTexto("Número de cuenta a cerrar: ");
            cooperativaImbabura.CerrarCuenta(numeroCuenta);
            Console.WriteLine("Cuenta cerrada.");
        }

        private static void EliminarCuenta()
        {
            string numeroCuenta = LeerTexto("Número de cuenta a eliminar: ");
            cooperativaImbabura.EliminarCuenta(numeroCuenta);
            Console.WriteLine("Listo. Si la cuenta tenía movimientos quedó INACTIVA; si no, se eliminó.");
        }


        private static void VerEstadoDeCuenta()
        {
            string numeroCuenta = LeerTexto("Número de cuenta: ");
            Console.WriteLine(cooperativaImbabura.ReporteEstadoDeCuenta(numeroCuenta));
        }

        private static void VerInteresesDelMes()
        {
            int anioDelReporte = LeerNumeroEntero("Año (ej. 2026): ");
            int mesDelReporte = LeerNumeroEntero("Mes (1-12): ");
            Console.WriteLine(cooperativaImbabura.ReporteInteresesDelMes(anioDelReporte, mesDelReporte));
        }

        private static void VerCuentasEnSobregiro()
        {
            Console.WriteLine(cooperativaImbabura.ReporteCuentasEnSobregiro());
        }

        private static void VerTotalesDelPeriodo()
        {
            DateTime fechaInicio = LeerFecha("Fecha de inicio (aaaa-mm-dd): ");
            DateTime fechaFin = LeerFecha("Fecha de fin (aaaa-mm-dd): ");
            fechaFin = fechaFin.AddDays(1).AddSeconds(-1);
            Console.WriteLine(cooperativaImbabura.ReporteTotalesDelPeriodo(fechaInicio, fechaFin));
        }

        private static void VerAuditoriaDeSaldos()
        {
            Console.WriteLine(cooperativaImbabura.ReporteAuditoriaDeSaldos());
        }


        private static string LeerTexto(string mensajeParaElUsuario)
        {
            Console.Write(mensajeParaElUsuario);
            string textoIngresado = Console.ReadLine() ?? "";
            return textoIngresado.Trim();
        }

        private static int LeerNumeroEntero(string mensajeParaElUsuario)
        {
            while (true)
            {
                string textoIngresado = LeerTexto(mensajeParaElUsuario);
                if (int.TryParse(textoIngresado, out int numeroIngresado))
                {
                    return numeroIngresado;
                }
                Console.WriteLine("Debe ingresar un número entero.");
            }
        }

        private static decimal LeerMonto(string mensajeParaElUsuario)
        {
            while (true)
            {
                string textoIngresado = LeerTexto(mensajeParaElUsuario).Replace(",", ".");
                if (decimal.TryParse(textoIngresado, NumberStyles.Number,
                                     CultureInfo.InvariantCulture, out decimal montoIngresado))
                {
                    return montoIngresado;
                }
                Console.WriteLine("Debe ingresar un monto válido, por ejemplo 25.50");
            }
        }

        private static DateTime LeerFecha(string mensajeParaElUsuario)
        {
            while (true)
            {
                string textoIngresado = LeerTexto(mensajeParaElUsuario);
                if (DateTime.TryParseExact(textoIngresado, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out DateTime fechaIngresada))
                {
                    return fechaIngresada;
                }
                Console.WriteLine("Fecha no válida. Use el formato aaaa-mm-dd, por ejemplo 2026-11-03");
            }
        }
    }
}