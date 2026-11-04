namespace CooperativaImbabura.Models
{

    public class CuentaCorriente : Cuenta
    {
        public const decimal LIMITE_SOBREGIRO = 200m;

        public CuentaCorriente(string numeroCuenta, string cedulaDelSocio)
            : base(numeroCuenta, cedulaDelSocio)
        {
        }

        public CuentaCorriente(string numeroCuenta, string cedulaDelSocio, decimal saldoGuardado, string estadoGuardado)
            : base(numeroCuenta, cedulaDelSocio, saldoGuardado, estadoGuardado)
        {
        }

        public override string ObtenerTipoCuenta()
        {
            return "CORRIENTE";
        }

        public override bool TieneFondosParaRetirar(decimal montoARetirar)
        {
            decimal saldoQueQuedaria = SaldoActual - montoARetirar;
            return saldoQueQuedaria >= -LIMITE_SOBREGIRO;
        }

        public bool EstaEnSobregiro()
        {
            return SaldoActual < 0;
        }
    }
}
