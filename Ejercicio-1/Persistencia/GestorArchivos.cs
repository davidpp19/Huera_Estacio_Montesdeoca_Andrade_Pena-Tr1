using Ejercicio_1.Modelos;
using Ejercicio_1.Servicios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace Ejercicio_1.Persistencia
{
    internal class MaterialJson
    {
        public String tipo { get; set; } = "";
        public String codigo { get; set; } = "";
        public String titulo { get; set; } = "";
        public int ejemplares { get; set; }
    }

    internal class UsuarioJson
    {
        public String id { get; set; } = "";
        public String nombre { get; set; } = "";
    }

    internal class PrestamoJson
    {
        public String idUsuario { get; set; } = "";
        public String codigoMaterial { get; set; } = "";
        public String fechaPrestamo { get; set; } = "";
        public String fechaDevolucion { get; set; }     
        public bool multaPagada { get; set; }
    }

    internal class OperacionJson
    {
        public String tipo { get; set; } = "";            
        public String idUsuario { get; set; }
        public String codigoMaterial { get; set; }
        public String idPrestamo { get; set; }
        public String fecha { get; set; }
    }

    internal class GestorArchivos
    {
        private static JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private static String carpetaDatos = "datos";
        private static List<OperacionJson> operaciones = new List<OperacionJson>();

        public static void cargar(Biblioteca biblioteca, String carpeta)
        {
            carpetaDatos = carpeta;
            cargarMateriales(biblioteca, Path.Combine(carpeta, "materiales.json"));
            cargarUsuarios(biblioteca, Path.Combine(carpeta, "usuarios.json"));
            cargarPrestamos(biblioteca, Path.Combine(carpeta, "prestamos.json"));
            cargarOperaciones(biblioteca, Path.Combine(carpeta, "operaciones.json"));
        }

        private static void cargarMateriales(Biblioteca biblioteca, String ruta)
        {
            String texto = File.ReadAllText(ruta);
            List<MaterialJson> lista = JsonSerializer.Deserialize<List<MaterialJson>>(texto, opciones);
            if (lista == null) return;

            foreach (MaterialJson mj in lista)
            {
                if (mj.tipo.ToUpper() == "LIBRO")
                {
                    biblioteca.agregarMaterial(new Libro(mj.codigo, mj.titulo, mj.ejemplares));
                }
                else if (mj.tipo.ToUpper() == "REVISTA")
                {
                    biblioteca.agregarMaterial(new Revista(mj.codigo, mj.titulo, mj.ejemplares));
                }
                else
                {
                    throw new InvalidOperationException("Tipo de material desconocido: " + mj.tipo);
                }
            }
        }

        private static void cargarUsuarios(Biblioteca biblioteca, String ruta)
        {
            String texto = File.ReadAllText(ruta);
            List<UsuarioJson> lista = JsonSerializer.Deserialize<List<UsuarioJson>>(texto, opciones);
            if (lista == null) return;

            foreach (UsuarioJson uj in lista)
            {
                biblioteca.agregarUsuario(new Usuario(uj.id, uj.nombre));
            }
        }

        private static void cargarPrestamos(Biblioteca biblioteca, String ruta)
        {
            String texto = File.ReadAllText(ruta);
            List<PrestamoJson> lista = JsonSerializer.Deserialize<List<PrestamoJson>>(texto, opciones);
            if (lista == null) return;

            int numero = 1;
            foreach (PrestamoJson pj in lista)
            {
                try
                {
                    DateTime fechaPrestamo = DateTime.Parse(pj.fechaPrestamo, CultureInfo.InvariantCulture);
                    biblioteca.prestar(pj.idUsuario, pj.codigoMaterial, fechaPrestamo);

                    Prestamo creado = biblioteca.prestamos[biblioteca.prestamos.Count - 1];

                    if (!String.IsNullOrWhiteSpace(pj.fechaDevolucion))
                    {
                        DateTime fechaDevolucion = DateTime.Parse(pj.fechaDevolucion, CultureInfo.InvariantCulture);
                        biblioteca.devolver(creado.id, fechaDevolucion);

                        if (pj.multaPagada && creado.multa > 0)
                        {
                            biblioteca.pagarMulta(creado.id);
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException(
                        "prestamos.json, registro " + numero + ": " + ex.Message);
                }
                numero++;
            }
        }

        private static void cargarOperaciones(Biblioteca biblioteca, String ruta)
        {
            if (!File.Exists(ruta)) return;   

            String texto = File.ReadAllText(ruta);
            List<OperacionJson> lista = JsonSerializer.Deserialize<List<OperacionJson>>(texto, opciones);
            if (lista == null) return;

            foreach (OperacionJson op in lista)
            {
                if (op.tipo == "PRESTAR")
                {
                    biblioteca.prestar(op.idUsuario, op.codigoMaterial,
                        DateTime.Parse(op.fecha, CultureInfo.InvariantCulture));
                }
                else if (op.tipo == "DEVOLVER")
                {
                    biblioteca.devolver(op.idPrestamo,
                        DateTime.Parse(op.fecha, CultureInfo.InvariantCulture));
                }
                else if (op.tipo == "PAGAR")
                {
                    biblioteca.pagarMulta(op.idPrestamo);
                }
                operaciones.Add(op);
            }
        }


        public static void registrarPrestamo(String idUsuario, String codigoMaterial, DateTime fecha)
        {
            OperacionJson op = new OperacionJson();
            op.tipo = "PRESTAR";
            op.idUsuario = idUsuario;
            op.codigoMaterial = codigoMaterial;
            op.fecha = fecha.ToString("yyyy-MM-dd");
            operaciones.Add(op);
            guardarOperaciones();
        }

        public static void registrarDevolucion(String idPrestamo, DateTime fecha)
        {
            OperacionJson op = new OperacionJson();
            op.tipo = "DEVOLVER";
            op.idPrestamo = idPrestamo;
            op.fecha = fecha.ToString("yyyy-MM-dd");
            operaciones.Add(op);
            guardarOperaciones();
        }

        public static void registrarPago(String idPrestamo)
        {
            OperacionJson op = new OperacionJson();
            op.tipo = "PAGAR";
            op.idPrestamo = idPrestamo;
            operaciones.Add(op);
            guardarOperaciones();
        }

        private static void guardarOperaciones()
        {
            JsonSerializerOptions conFormato = new JsonSerializerOptions { WriteIndented = true };
            String texto = JsonSerializer.Serialize(operaciones, conFormato);
            File.WriteAllText(Path.Combine(carpetaDatos, "operaciones.json"), texto);
        }
    }
}