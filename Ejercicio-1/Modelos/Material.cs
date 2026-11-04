using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_1.Modelos
{
    public abstract class Material
    {
        public String codigo { get; private set; }
        public String titulo { get; private set; }
        public int ejemplaresTotales { get; private set; }
        public int ejemplaresDisponibles { get; private set; }
        public abstract int diasPrestamo { get; }
        public abstract decimal multaDiaria { get; }

        protected Material(String codigo, String titulo, int ejemplaresTotales)
        {
            this.codigo = codigo;
            this.titulo = titulo;
            if (ejemplaresTotales < 1)
            {
                throw new ArgumentException("Debe haber al menos 1 ejemplar.");
            }
            else {
                this.ejemplaresTotales = ejemplaresTotales;
            }
            
            this.ejemplaresDisponibles = ejemplaresTotales;
        }
        public bool hayDisponibles() {
            if (this.ejemplaresDisponibles > 0)
            {
                return true;
            }
            else { 
                return false;
            }
        }
        public void prestar()
        {
            if (ejemplaresDisponibles <= 0)
            {
                throw new InvalidOperationException("No hay ejemplares disponibles para prestar.");
            }
            else {
                ejemplaresDisponibles-=1;
            }
        }
        public void devolver() {
            if (ejemplaresDisponibles == ejemplaresTotales) { 
                throw new InvalidOperationException("No se puede devolver un ejemplar que no ha sido prestado.");
            }
            else
            {
                ejemplaresDisponibles++;
            }
        }


    }
}
