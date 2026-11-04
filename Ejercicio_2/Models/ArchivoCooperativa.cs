using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CooperativaImbabura.Models
{
    public class ArchivoCooperativa
    {
        private const char SEPARADOR = ';';
        private const string FORMATO_FECHA = "yyyy-MM-dd HH:mm:ss";

        private readonly CultureInfo formatoNumeros = CultureInfo.InvariantCulture;

        private readonly string carpetaDeDatos;
        private readonly string rutaArchivoSocios;
        private readonly string rutaArchivoCuentas;
        private readonly string rutaArchivoMovimientos;

        public ArchivoCooperativa(string carpetaDeDatos)
        {
            this.carpetaDeDatos = carpetaDeDatos;
            Directory.CreateDirectory(carpetaDeDatos);

            rutaArchivoSocios = Path.Combine(carpetaDeDatos, "socios.txt");
            rutaArchivoCuentas = Path.Combine(carpetaDeDatos, "cuentas.txt");
            rutaArchivoMovimientos = Path.Combine(carpetaDeDatos, "movimientos.txt");
        }


        public void GuardarSocios(List<Socio> listaDeSocios)
        {
            List<string> lineasDelArchivo = new List<string>();
            foreach (Socio socio in listaDeSocios)
            {
                lineasDelArchivo.Add(socio.CedulaSocio + SEPARADOR + socio.NombreCompletoSocio);
            }
            File.WriteAllLines(rutaArchivoSocios, lineasDelArchivo);
        }

        public List<Socio> LeerSocios()
        {
            List<Socio> listaDeSocios = new List<Socio>();
            if (!File.Exists(rutaArchivoSocios))
            {
                return listaDeSocios;
            }

            foreach (string lineaDelArchivo in File.ReadAllLines(rutaArchivoSocios))
            {
                if (lineaDelArchivo.Trim() == "") continue;

                string[] datosDelSocio = lineaDelArchivo.Split(SEPARADOR);
                Socio socioLeido = new Socio(datosDelSocio[0], datosDelSocio[1]);
                listaDeSocios.Add(socioLeido);
            }
            return listaDeSocios;
        }


        public void GuardarCuentas(List<Cuenta> listaDeCuentas)
        {
            List<string> lineasDelArchivo = new List<string>();
            foreach (Cuenta cuenta in listaDeCuentas)
            {
                string lineaDeCuenta = cuenta.ObtenerTipoCuenta() + SEPARADOR +
                                       cuenta.NumeroCuenta + SEPARADOR +
                                       cuenta.CedulaDelSocio + SEPARADOR +
                                       cuenta.SaldoActual.ToString(formatoNumeros) + SEPARADOR +
                                       cuenta.EstadoCuenta;
                lineasDelArchivo.Add(lineaDeCuenta);
            }
            File.WriteAllLines(rutaArchivoCuentas, lineasDelArchivo);
        }

        public List<Cuenta> LeerCuentas()
        {
            List<Cuenta> listaDeCuentas = new List<Cuenta>();
            if (!File.Exists(rutaArchivoCuentas))
            {
                return listaDeCuentas;
            }

            foreach (string lineaDelArchivo in File.ReadAllLines(rutaArchivoCuentas))
            {
                if (lineaDelArchivo.Trim() == "") continue;

                string[] datosDeCuenta = lineaDelArchivo.Split(SEPARADOR);
                string tipoCuenta = datosDeCuenta[0];
                string numeroCuenta = datosDeCuenta[1];
                string cedulaDelSocio = datosDeCuenta[2];
                decimal saldoGuardado = decimal.Parse(datosDeCuenta[3], formatoNumeros);
                string estadoGuardado = datosDeCuenta[4];

                if (tipoCuenta == "AHORROS")
                {
                    listaDeCuentas.Add(new CuentaAhorros(numeroCuenta, cedulaDelSocio, saldoGuardado, estadoGuardado));
                }
                else
                {
                    listaDeCuentas.Add(new CuentaCorriente(numeroCuenta, cedulaDelSocio, saldoGuardado, estadoGuardado));
                }
            }
            return listaDeCuentas;
        }

        public void GuardarMovimientos(List<Cuenta> listaDeCuentas)
        {
            List<string> lineasDelArchivo = new List<string>();
            foreach (Cuenta cuenta in listaDeCuentas)
            {
                foreach (Movimiento movimiento in cuenta.MovimientosDeLaCuenta)
                {
                    string lineaDeMovimiento = movimiento.NumeroCuenta + SEPARADOR +
                                               movimiento.FechaMovimiento.ToString(FORMATO_FECHA) + SEPARADOR +
                                               movimiento.TipoMovimiento + SEPARADOR +
                                               movimiento.MontoMovimiento.ToString(formatoNumeros) + SEPARADOR +
                                               movimiento.SaldoDespuesDelMovimiento.ToString(formatoNumeros) + SEPARADOR +
                                               movimiento.DescripcionMovimiento.Replace(";", ",");
                    lineasDelArchivo.Add(lineaDeMovimiento);
                }
            }
            File.WriteAllLines(rutaArchivoMovimientos, lineasDelArchivo);
        }

        public void LeerMovimientos(List<Cuenta> listaDeCuentas)
        {
            if (!File.Exists(rutaArchivoMovimientos))
            {
                return;
            }

            foreach (string lineaDelArchivo in File.ReadAllLines(rutaArchivoMovimientos))
            {
                if (lineaDelArchivo.Trim() == "") continue;

                string[] datosDelMovimiento = lineaDelArchivo.Split(SEPARADOR);
                Movimiento movimientoLeido = new Movimiento(
                    datosDelMovimiento[0],
                    DateTime.ParseExact(datosDelMovimiento[1], FORMATO_FECHA, formatoNumeros),
                    datosDelMovimiento[2],
                    decimal.Parse(datosDelMovimiento[3], formatoNumeros),
                    decimal.Parse(datosDelMovimiento[4], formatoNumeros),
                    datosDelMovimiento[5]);

                foreach (Cuenta cuenta in listaDeCuentas)
                {
                    if (cuenta.NumeroCuenta == movimientoLeido.NumeroCuenta)
                    {
                        cuenta.AgregarMovimientoGuardado(movimientoLeido);
                        break;
                    }
                }
            }
        }

        public void GuardarReporte(string nombreDelReporte, string textoDelReporte)
        {
            string rutaDelReporte = Path.Combine(carpetaDeDatos, nombreDelReporte + ".txt");
            File.WriteAllText(rutaDelReporte, textoDelReporte);
        }
    }
}
