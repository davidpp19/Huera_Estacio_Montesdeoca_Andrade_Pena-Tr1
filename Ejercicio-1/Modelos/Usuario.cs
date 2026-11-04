using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_1.Modelos
{
    internal class Usuario
    {
        public String id { get; private set; }
        public String nombre { get; private set; }
        public List<Prestamo> prestamo { get; private set; }
        public Usuario(String id, String nombre)
        {
            this.id = id;
            this.nombre = nombre;
            this.prestamo = new List<Prestamo>();
        }
        public int prestamosActivos() {
            int contador = 0;
            foreach (Prestamo p in prestamo) {
                if (!p.estaDevuelto()) {
                    contador++;
                }
            }
            return contador;
        }
        public bool tieneMultasPendientes() {
            foreach (Prestamo p in prestamo) {
                if (p.multa > 0 && p.multaPagada == false)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
