using System;

namespace CooperativaImbabura.Models
{
    public class CuentaAhorros : Cuenta
    {
        public const decimal TASA_INTERES_ANUAL = 0.04m;
        public const decimal SALDO_MINIMO = 5m;

        public CuentaAhorros(string numeroCuenta, string cedulaDelSocio)
            : base(numeroCuenta, cedulaDelSocio)
        {
        }

        public CuentaAhorros(string numeroCuenta, string cedulaDelSocio, decimal saldoGuardado, string estadoGuardado)
            : base(numeroCuenta, cedulaDelSocio, saldoGuardado, estadoGuardado)
        {
        }

        public override string ObtenerTipoCuenta()
        {
            return "AHORROS";
        }

        public override bool TieneFondosParaRetirar(decimal montoARetirar)
        {
            decimal saldoQueQuedaria = SaldoActual - montoARetirar;
            return saldoQueQuedaria >= SALDO_MINIMO;
        }

        public decimal CalcularInteresDelMes()
        {
            if (SaldoActual <= 0)
            {
                return 0;
            }
            decimal interesDelMes = SaldoActual * TASA_INTERES_ANUAL / 12;
            return Math.Round(interesDelMes, 2);
        }

        public decimal AplicarInteresDelMes()
        {
            decimal interesDelMes = CalcularInteresDelMes();
            if (interesDelMes > 0 && EstaActiva())
            {
                SumarInteres(interesDelMes, "Interés mensual");
            }
            return interesDelMes;
        }
    }
}
