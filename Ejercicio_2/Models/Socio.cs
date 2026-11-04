using System.Collections.Generic;

namespace CooperativaImbabura.Models
{
    public class Socio
    {
        public string CedulaSocio { get; private set; }
        public string NombreCompletoSocio { get; private set; }
        public List<Cuenta> CuentasDelSocio { get; private set; }

        public Socio(string cedulaSocio, string nombreCompletoSocio)
        {
            CedulaSocio = cedulaSocio;
            NombreCompletoSocio = nombreCompletoSocio;
            CuentasDelSocio = new List<Cuenta>();
        }

        public void AgregarCuenta(Cuenta cuentaNueva)
        {
            CuentasDelSocio.Add(cuentaNueva);
        }
    }
}
