using System;

namespace CooperativaImbabura.Models
{
    public class Movimiento
    {
        public const string TIPO_DEPOSITO = "DEPOSITO";
        public const string TIPO_RETIRO = "RETIRO";
        public const string TIPO_INTERES = "INTERES";

        public string NumeroCuenta { get; private set; }
        public DateTime FechaMovimiento { get; private set; }
        public string TipoMovimiento { get; private set; }
        public decimal MontoMovimiento { get; private set; }
        public decimal SaldoDespuesDelMovimiento { get; private set; }
        public string DescripcionMovimiento { get; private set; }

        public Movimiento(string numeroCuenta, DateTime fechaMovimiento, string tipoMovimiento,
                          decimal montoMovimiento, decimal saldoDespuesDelMovimiento,
                          string descripcionMovimiento)
        {
            NumeroCuenta = numeroCuenta;
            FechaMovimiento = fechaMovimiento;
            TipoMovimiento = tipoMovimiento;
            MontoMovimiento = montoMovimiento;
            SaldoDespuesDelMovimiento = saldoDespuesDelMovimiento;
            DescripcionMovimiento = descripcionMovimiento;
        }
        public decimal ObtenerMontoConSigno()
        {
            if (TipoMovimiento == TIPO_RETIRO)
            {
                return -MontoMovimiento;
            }
            return MontoMovimiento;
        }
    }
}
