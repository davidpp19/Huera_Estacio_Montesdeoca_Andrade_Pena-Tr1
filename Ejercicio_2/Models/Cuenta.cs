using System;
using System.Collections.Generic;

namespace CooperativaImbabura.Models
{

    public abstract class Cuenta
    {
        public const string ESTADO_ACTIVA = "ACTIVA";
        public const string ESTADO_CERRADA = "CERRADA";
        public const string ESTADO_INACTIVA = "INACTIVA";

        public string NumeroCuenta { get; private set; }
        public string CedulaDelSocio { get; private set; }
        public decimal SaldoActual { get; private set; }
        public string EstadoCuenta { get; private set; }
        public List<Movimiento> MovimientosDeLaCuenta { get; private set; }

        public Cuenta(string numeroCuenta, string cedulaDelSocio)
        {
            NumeroCuenta = numeroCuenta;
            CedulaDelSocio = cedulaDelSocio;
            SaldoActual = 0;
            EstadoCuenta = ESTADO_ACTIVA;
            MovimientosDeLaCuenta = new List<Movimiento>();
        }

        public Cuenta(string numeroCuenta, string cedulaDelSocio, decimal saldoGuardado, string estadoGuardado)
        {
            NumeroCuenta = numeroCuenta;
            CedulaDelSocio = cedulaDelSocio;
            SaldoActual = saldoGuardado;
            EstadoCuenta = estadoGuardado;
            MovimientosDeLaCuenta = new List<Movimiento>();
        }

        public abstract string ObtenerTipoCuenta();

        public abstract bool TieneFondosParaRetirar(decimal montoARetirar);

        public bool EstaActiva()
        {
            return EstadoCuenta == ESTADO_ACTIVA;
        }

        public bool TieneMovimientos()
        {
            return MovimientosDeLaCuenta.Count > 0;
        }

        public bool SePuedeRetirar(decimal montoARetirar)
        {
            return montoARetirar > 0 && EstaActiva() && TieneFondosParaRetirar(montoARetirar);
        }

        public bool SePuedeDepositar(decimal montoADepositar)
        {
            return montoADepositar > 0 && EstaActiva();
        }

        public void Depositar(decimal montoADepositar, string descripcionDeposito)
        {
            if (montoADepositar <= 0)
            {
                throw new Exception("El depósito debe ser mayor que cero.");
            }
            if (!EstaActiva())
            {
                throw new Exception("La cuenta " + NumeroCuenta + " no está activa.");
            }

            SaldoActual = SaldoActual + montoADepositar;
            RegistrarMovimiento(Movimiento.TIPO_DEPOSITO, montoADepositar, descripcionDeposito);
        }

        public void Retirar(decimal montoARetirar, string descripcionRetiro)
        {
            if (montoARetirar <= 0)
            {
                throw new Exception("El retiro debe ser mayor que cero.");
            }
            if (!EstaActiva())
            {
                throw new Exception("La cuenta " + NumeroCuenta + " no está activa.");
            }
            if (!TieneFondosParaRetirar(montoARetirar))
            {
                throw new Exception("Fondos insuficientes en la cuenta " + NumeroCuenta + ".");
            }

            SaldoActual = SaldoActual - montoARetirar;
            RegistrarMovimiento(Movimiento.TIPO_RETIRO, montoARetirar, descripcionRetiro);
        }

        protected void SumarInteres(decimal montoInteres, string descripcionInteres)
        {
            SaldoActual = SaldoActual + montoInteres;
            RegistrarMovimiento(Movimiento.TIPO_INTERES, montoInteres, descripcionInteres);
        }

        private void RegistrarMovimiento(string tipoMovimiento, decimal montoMovimiento, string descripcionMovimiento)
        {
            Movimiento movimientoNuevo = new Movimiento(NumeroCuenta, DateTime.Now, tipoMovimiento,
                                                        montoMovimiento, SaldoActual, descripcionMovimiento);
            MovimientosDeLaCuenta.Add(movimientoNuevo);
        }

        public void AgregarMovimientoGuardado(Movimiento movimientoGuardado)
        {
            MovimientosDeLaCuenta.Add(movimientoGuardado);
        }

        public void Cerrar()
        {
            if (SaldoActual != 0)
            {
                throw new Exception("No se puede cerrar la cuenta " + NumeroCuenta +
                                    " porque su saldo es " + SaldoActual + ".");
            }
            EstadoCuenta = ESTADO_CERRADA;
        }

        public void MarcarComoInactiva()
        {
            EstadoCuenta = ESTADO_INACTIVA;
        }
    }
}
