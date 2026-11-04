using Ejercicio_5.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Ejercicio_5.Services
{
    internal class ArchivoServicio
    {
        // Guardar información en JSON
        public void Guardar(string ruta, DatosTienda datos)
        {
            string carpeta = Path.GetDirectoryName(
                Path.GetFullPath(ruta));

            Directory.CreateDirectory(carpeta);

            JsonSerializerOptions opciones =
                new JsonSerializerOptions
                {
                    WriteIndented = true
                };

            string json = JsonSerializer.Serialize(
                datos, opciones);

            File.WriteAllText(ruta, json);
        }

        // Recuperar información del archivo
        public DatosTienda Cargar(string ruta)
        {
            if (!File.Exists(ruta))
            {
                throw new FileNotFoundException(
                    "No existe el archivo de datos.", ruta);
            }

            string json = File.ReadAllText(ruta);

            DatosTienda datos =
                JsonSerializer.Deserialize<DatosTienda>(json);

            if (datos == null ||
                datos.Artesanos == null ||
                datos.Artesanias == null ||
                datos.Lotes == null ||
                datos.Movimientos == null)
            {
                throw new InvalidDataException(
                    "El archivo contiene datos no válidos.");
            }

            return datos;
        }
    }
}
