using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_1.Modelos
{
    internal class Prestamo
    {
        public String id { get; private set; }
        public int diasRetraso { get; private set; }
        public Material material { get; private set; }
        public Usuario usuario { get; private set; }
        public DateTime fechaPrestamo { get; private set; }
        public DateTime fechaVencimiento { get; private set; }
        public DateTime? fechaDevolucion { get; private set; }
        public decimal multa { get; private set; }
        public bool multaPagada { get; private set; }

        public Prestamo(String id, Material material, Usuario usuario, DateTime fechaPrestamo)
        {
            this.id = id;
            this.material = material;
            this.usuario = usuario;
            this.fechaPrestamo = fechaPrestamo;
            this.fechaVencimiento = fechaPrestamo.AddDays(material.diasPrestamo);
            this.fechaDevolucion = null;
            this.multa = 0;
            this.multaPagada = false;
        }
        public bool estaDevuelto() {
            if (this.fechaDevolucion == null) {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void registrarDevolucion(DateTime fecha) {
            if (estaDevuelto())
            {
                throw new InvalidOperationException("El articulo ya se ha devuelto");
            }
            else {
                diasRetraso = (fecha - fechaVencimiento).Days;
                if (diasRetraso < 0) {
                    diasRetraso = 0;
                }
                this.multa = diasRetraso * material.multaDiaria;
                this.fechaDevolucion = fecha;
            }
        }
        public void pagarMulta(){
            if (multa == 0)
            {
                throw new InvalidOperationException("No hay multa que pagar.");
            }
            else if (multaPagada == true)
            {
                throw new InvalidOperationException("multa ya pagada");
            }
            else {
                multaPagada = true;
            }
        }
        
    }
}
