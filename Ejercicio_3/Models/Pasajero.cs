using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_3.Models
{
    internal class Pasajero
    {
        public string Nombre;
        public string Cedula;
        public string Tipo;   // "Regular", "Tercera edad" o "Discapacidad"

        public Pasajero(string nombre, string cedula, string tipo)
        {
            Nombre = nombre;
            Cedula = cedula;
            Tipo = tipo;
        }
    }
}
