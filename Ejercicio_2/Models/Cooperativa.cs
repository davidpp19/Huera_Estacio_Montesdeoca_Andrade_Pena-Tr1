using System;
using System.Collections.Generic;

namespace CooperativaImbabura.Models
{

    public class Cooperativa
    {
        private readonly List<Socio> listaDeSocios;
        private readonly List<Cuenta> listaDeCuentas;
        private readonly ArchivoCooperativa archivoCooperativa;

 
        public Cooperativa(string carpetaDeDatos)
        {
            archivoCooperativa = new ArchivoCooperativa(carpetaDeDatos);
            listaDeSocios = archivoCooperativa.LeerSocios();
            listaDeCuentas = archivoCooperativa.LeerCuentas();
            archivoCooperativa.LeerMovimientos(listaDeCuentas);

            foreach (Cuenta cuenta in listaDeCuentas)
            {
                Socio duenoDeLaCuenta = BuscarSocio(cuenta.CedulaDelSocio);
                if (duenoDeLaCuenta != null)
                {
                    duenoDeLaCuenta.AgregarCuenta(cuenta);
                }
            }
        }

        private void GuardarTodo()
        {
            archivoCooperativa.GuardarSocios(listaDeSocios);
            archivoCooperativa.GuardarCuentas(listaDeCuentas);
            archivoCooperativa.GuardarMovimientos(listaDeCuentas);
        }

        public Socio BuscarSocio(string cedulaSocio)
        {
            foreach (Socio socio in listaDeSocios)
            {
                if (socio.CedulaSocio == cedulaSocio)
                {
                    return socio;
                }
            }
            return null;
        }

        public Cuenta BuscarCuenta(string numeroCuenta)
        {
            foreach (Cuenta cuenta in listaDeCuentas)
            {
                if (cuenta.NumeroCuenta == numeroCuenta)
                {
                    return cuenta;
                }
            }
            return null;
        }

        private Cuenta BuscarCuentaObligatoria(string numeroCuenta)
        {
            Cuenta cuentaEncontrada = BuscarCuenta(numeroCuenta);
            if (cuentaEncontrada == null)
            {
                throw new Exception("No existe la cuenta " + numeroCuenta + ".");
            }
            return cuentaEncontrada;
        }

        public Socio RegistrarSocio(string cedulaSocio, string nombreCompletoSocio)
        {
            if (BuscarSocio(cedulaSocio) != null)
            {
                throw new Exception("Ya existe un socio con la cédula " + cedulaSocio + ".");
            }
            Socio socioNuevo = new Socio(cedulaSocio, nombreCompletoSocio);
            listaDeSocios.Add(socioNuevo);
            GuardarTodo();
            return socioNuevo;
        }

        public CuentaAhorros AbrirCuentaAhorros(string cedulaSocio)
        {
            Socio socio = BuscarSocio(cedulaSocio);
            if (socio == null)
            {
                throw new Exception("No existe el socio con cédula " + cedulaSocio + ".");
            }
            string numeroCuentaNueva = "AH-" + (listaDeCuentas.Count + 1).ToString("0000");
            CuentaAhorros cuentaNueva = new CuentaAhorros(numeroCuentaNueva, cedulaSocio);
            listaDeCuentas.Add(cuentaNueva);
            socio.AgregarCuenta(cuentaNueva);
            GuardarTodo();
            return cuentaNueva;
        }

        public CuentaCorriente AbrirCuentaCorriente(string cedulaSocio)
        {
            Socio socio = BuscarSocio(cedulaSocio);
            if (socio == null)
            {
                throw new Exception("No existe el socio con cédula " + cedulaSocio + ".");
            }
            string numeroCuentaNueva = "CC-" + (listaDeCuentas.Count + 1).ToString("0000");
            CuentaCorriente cuentaNueva = new CuentaCorriente(numeroCuentaNueva, cedulaSocio);
            listaDeCuentas.Add(cuentaNueva);
            socio.AgregarCuenta(cuentaNueva);
            GuardarTodo();
            return cuentaNueva;
        }

        public void CerrarCuenta(string numeroCuenta)
        {
            Cuenta cuentaACerrar = BuscarCuentaObligatoria(numeroCuenta);
            cuentaACerrar.Cerrar();
            GuardarTodo();
        }

        
        public void EliminarCuenta(string numeroCuenta)
        {
            Cuenta cuentaAEliminar = BuscarCuentaObligatoria(numeroCuenta);
            if (cuentaAEliminar.SaldoActual != 0)
            {
                throw new Exception("No se puede eliminar una cuenta con saldo distinto de cero.");
            }

            if (cuentaAEliminar.TieneMovimientos())
            {
                cuentaAEliminar.MarcarComoInactiva();
            }
            else
            {
                listaDeCuentas.Remove(cuentaAEliminar);
                Socio duenoDeLaCuenta = BuscarSocio(cuentaAEliminar.CedulaDelSocio);
                if (duenoDeLaCuenta != null)
                {
                    duenoDeLaCuenta.CuentasDelSocio.Remove(cuentaAEliminar);
                }
            }
            GuardarTodo();
        }

        public void Depositar(string numeroCuenta, decimal montoADepositar)
        {
            Cuenta cuenta = BuscarCuentaObligatoria(numeroCuenta);
            cuenta.Depositar(montoADepositar, "Depósito");
            GuardarTodo();
        }

        public void Retirar(string numeroCuenta, decimal montoARetirar)
        {
            Cuenta cuenta = BuscarCuentaObligatoria(numeroCuenta);
            cuenta.Retirar(montoARetirar, "Retiro");
            GuardarTodo();
        }

        public void Transferir(string numeroCuentaOrigen, string numeroCuentaDestino, decimal montoATransferir)
        {
            if (numeroCuentaOrigen == numeroCuentaDestino)
            {
                throw new Exception("No se puede transferir a la misma cuenta.");
            }

            Cuenta cuentaOrigen = BuscarCuentaObligatoria(numeroCuentaOrigen);
            Cuenta cuentaDestino = BuscarCuentaObligatoria(numeroCuentaDestino);

            if (!cuentaOrigen.SePuedeRetirar(montoATransferir))
            {
                throw new Exception("Transferencia rechazada: no se puede retirar de la cuenta " + numeroCuentaOrigen + ".");
            }
            if (!cuentaDestino.SePuedeDepositar(montoATransferir))
            {
                throw new Exception("Transferencia rechazada: no se puede depositar en la cuenta " + numeroCuentaDestino + ".");
            }

            cuentaOrigen.Retirar(montoATransferir, "Transferencia a " + numeroCuentaDestino);
            cuentaDestino.Depositar(montoATransferir, "Transferencia de " + numeroCuentaOrigen);
            GuardarTodo();
        }

        public void AplicarInteresesDelMes()
        {
            foreach (Cuenta cuenta in listaDeCuentas)
            {
                if (cuenta is CuentaAhorros cuentaDeAhorros)
                {
                    cuentaDeAhorros.AplicarInteresDelMes();
                }
            }
            GuardarTodo();
        }

        public string ReporteEstadoDeCuenta(string numeroCuenta)
        {
            Cuenta cuenta = BuscarCuentaObligatoria(numeroCuenta);
            Socio duenoDeLaCuenta = BuscarSocio(cuenta.CedulaDelSocio);

            string textoDelReporte = "ESTADO DE CUENTA\n";
            textoDelReporte += "Cuenta: " + cuenta.NumeroCuenta + " (" + cuenta.ObtenerTipoCuenta() + ")\n";
            textoDelReporte += "Socio: " + duenoDeLaCuenta.NombreCompletoSocio + "\n";
            textoDelReporte += "Estado: " + cuenta.EstadoCuenta + "\n\n";
            textoDelReporte += "FECHA               | TIPO      | MONTO      | SALDO      | DESCRIPCIÓN\n";

            foreach (Movimiento movimiento in cuenta.MovimientosDeLaCuenta)
            {
                textoDelReporte += movimiento.FechaMovimiento.ToString("yyyy-MM-dd HH:mm:ss") + " | "
                                 + movimiento.TipoMovimiento.PadRight(9) + " | "
                                 + movimiento.ObtenerMontoConSigno().ToString("0.00").PadLeft(10) + " | "
                                 + movimiento.SaldoDespuesDelMovimiento.ToString("0.00").PadLeft(10) + " | "
                                 + movimiento.DescripcionMovimiento + "\n";
            }
            textoDelReporte += "\nSaldo actual: " + cuenta.SaldoActual.ToString("0.00") + "\n";

            archivoCooperativa.GuardarReporte("estado_cuenta_" + numeroCuenta, textoDelReporte);
            return textoDelReporte;
        }

        public string ReporteInteresesDelMes(int anioDelReporte, int mesDelReporte)
        {
            string textoDelReporte = "INTERESES DEL MES " + mesDelReporte + "/" + anioDelReporte + "\n";
            decimal totalDeIntereses = 0;

            foreach (Cuenta cuenta in listaDeCuentas)
            {
                if (cuenta is CuentaAhorros)
                {
                    decimal interesDeLaCuenta = 0;
                    foreach (Movimiento movimiento in cuenta.MovimientosDeLaCuenta)
                    {
                        bool esInteres = movimiento.TipoMovimiento == Movimiento.TIPO_INTERES;
                        bool esDelMes = movimiento.FechaMovimiento.Year == anioDelReporte
                                     && movimiento.FechaMovimiento.Month == mesDelReporte;
                        if (esInteres && esDelMes)
                        {
                            interesDeLaCuenta += movimiento.MontoMovimiento;
                        }
                    }
                    textoDelReporte += cuenta.NumeroCuenta + ": " + interesDeLaCuenta.ToString("0.00") + "\n";
                    totalDeIntereses += interesDeLaCuenta;
                }
            }
            textoDelReporte += "Total de intereses: " + totalDeIntereses.ToString("0.00") + "\n";

            archivoCooperativa.GuardarReporte("intereses_" + anioDelReporte + "_" + mesDelReporte, textoDelReporte);
            return textoDelReporte;
        }

        public string ReporteCuentasEnSobregiro()
        {
            string textoDelReporte = "CUENTAS EN SOBREGIRO\n";
            int cantidadEnSobregiro = 0;

            foreach (Cuenta cuenta in listaDeCuentas)
            {
                if (cuenta is CuentaCorriente cuentaCorriente && cuentaCorriente.EstaEnSobregiro())
                {
                    Socio duenoDeLaCuenta = BuscarSocio(cuenta.CedulaDelSocio);
                    textoDelReporte += cuenta.NumeroCuenta + " - " + duenoDeLaCuenta.NombreCompletoSocio
                                     + " - Saldo: " + cuenta.SaldoActual.ToString("0.00") + "\n";
                    cantidadEnSobregiro++;
                }
            }
            textoDelReporte += "Cuentas en sobregiro: " + cantidadEnSobregiro + "\n";

            archivoCooperativa.GuardarReporte("cuentas_en_sobregiro", textoDelReporte);
            return textoDelReporte;
        }

        public string ReporteTotalesDelPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            decimal totalDepositado = 0;
            decimal totalRetirado = 0;

            foreach (Cuenta cuenta in listaDeCuentas)
            {
                foreach (Movimiento movimiento in cuenta.MovimientosDeLaCuenta)
                {
                    bool estaEnElPeriodo = movimiento.FechaMovimiento >= fechaInicio
                                        && movimiento.FechaMovimiento <= fechaFin;
                    if (!estaEnElPeriodo) continue;

                    if (movimiento.TipoMovimiento == Movimiento.TIPO_DEPOSITO)
                    {
                        totalDepositado += movimiento.MontoMovimiento;
                    }
                    else if (movimiento.TipoMovimiento == Movimiento.TIPO_RETIRO)
                    {
                        totalRetirado += movimiento.MontoMovimiento;
                    }
                }
            }

            string textoDelReporte = "TOTALES DEL " + fechaInicio.ToString("yyyy-MM-dd")
                                   + " AL " + fechaFin.ToString("yyyy-MM-dd") + "\n";
            textoDelReporte += "Total depositado: " + totalDepositado.ToString("0.00") + "\n";
            textoDelReporte += "Total retirado: " + totalRetirado.ToString("0.00") + "\n";

            archivoCooperativa.GuardarReporte("totales_del_periodo", textoDelReporte);
            return textoDelReporte;
        }
        public string ReporteAuditoriaDeSaldos()
        {
            string textoDelReporte = "AUDITORÍA DE SALDOS\n";
            int cuentasConError = 0;

            foreach (Cuenta cuenta in listaDeCuentas)
            {
                decimal saldoRecalculado = 0;
                foreach (Movimiento movimiento in cuenta.MovimientosDeLaCuenta)
                {
                    saldoRecalculado += movimiento.ObtenerMontoConSigno();
                }

                bool saldoCuadra = saldoRecalculado == cuenta.SaldoActual;
                string resultadoAuditoria = saldoCuadra ? "CORRECTO" : "NO CUADRA";
                if (!saldoCuadra) cuentasConError++;

                textoDelReporte += cuenta.NumeroCuenta
                                 + " | Saldo guardado: " + cuenta.SaldoActual.ToString("0.00")
                                 + " | Saldo recalculado: " + saldoRecalculado.ToString("0.00")
                                 + " | " + resultadoAuditoria + "\n";
            }
            textoDelReporte += "Cuentas con diferencias: " + cuentasConError + "\n";

            archivoCooperativa.GuardarReporte("auditoria_de_saldos", textoDelReporte);
            return textoDelReporte;
        }
    }
}
