
using Ejercicio_5.Models;
using Ejercicio_5.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ejercicio_5.Servicios
{
    public class TiendaServicio
    {
        // listas para almacenar la información
        private readonly List<Artesano> artesanos =
            new List<Artesano>();

        private readonly List<Artesania> artesanias =
            new List<Artesania>();

        private readonly List<LotesConsignacion> lotes =
            new List<LotesConsignacion>();

        private readonly List<Movimiento> movimientos =
            new List<Movimiento>();

        // primero registramos un artesano
        public void RegistrarArtesano(Artesano artesano)
        {
            if (artesano == null ||
                artesano.Id <= 0 ||
                string.IsNullOrWhiteSpace(artesano.Nombre) ||
                string.IsNullOrWhiteSpace(artesano.Comunidad))
            {
                throw new ArgumentException(
                    "Datos del artesano no válidos.");
            }

            if (artesanos.Any(a => a.Id == artesano.Id))
            {
                throw new InvalidOperationException(
                    "El artesano ya está registrado.");
            }

            artesanos.Add(artesano);
        }

        // segundo registramos la artesanía
        public void RegistrarArtesania(Artesania artesania)
        {
            if (artesania == null ||
                artesania.Id <= 0 ||
                string.IsNullOrWhiteSpace(artesania.Nombre) ||
                artesania.PrecioVenta <= 0 ||
                !Enum.IsDefined(typeof(TipoArtesania),
                                artesania.Tipo))
            {
                throw new ArgumentException(
                    "Datos de la artesanía no válidos.");
            }

            if (!artesanos.Any(a => a.Id == artesania.ArtesanoId))
            {
                throw new InvalidOperationException(
                    "El artesano propietario no existe.");
            }

            if (artesanias.Any(a => a.Id == artesania.Id))
            {
                throw new InvalidOperationException(
                    "La artesanía ya está registrada.");
            }

            artesanias.Add(artesania);
        }

        // tercero recibimos la artesania
        public void RecibirArtesania(
            int idLote,
            int artesaniaId,
            int cantidad,
            DateTime fecha)
        {
            if (idLote <= 0 || cantidad <= 0 ||
                fecha.Date > DateTime.Today)
            {
                throw new ArgumentException(
                    "Datos de recepción no válidos.");
            }

            Artesania artesania = artesanias.FirstOrDefault(
                a => a.Id == artesaniaId);

            if (artesania == null)
            {
                throw new InvalidOperationException(
                    "La artesanía no existe.");
            }

            if (lotes.Any(l => l.Id == idLote))
            {
                throw new InvalidOperationException(
                    "El lote ya está registrado.");
            }

            // crea el lote
            LotesConsignacion lote = new LotesConsignacion
            {
                Id = idLote,
                ArtesaniaId = artesaniaId,
                FechaRecepcion = fecha,
                CantidadRecibida = cantidad,
                CantidadDisponible = cantidad
            };

            // registra la recepcion
            Movimiento movimiento = new Movimiento
            {
                Id = movimientos.Count + 1,
                ArtesaniaId = artesaniaId,
                ArtesanoId = artesania.ArtesanoId,
                LoteId = idLote,
                Tipo = TipoMovimiento.Recepcion,
                Fecha = fecha,
                Cantidad = cantidad,
                PrecioUnitario = artesania.PrecioVenta,
                ComisionTotal = 0m
            };

            lotes.Add(lote);
            movimientos.Add(movimiento);
        }

        public decimal VenderArtesania(int artesaniaId, int cantidad)
        {
            // Validar cantidad
            if (cantidad <= 0)
            {
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");
            }

            // Buscar artesanía
            Artesania artesania = artesanias.FirstOrDefault(
                a => a.Id == artesaniaId);

            if (artesania == null)
            {
                throw new InvalidOperationException(
                    "La artesanía no existe.");
            }

            // Verificar stock antes de vender
            if (ObtenerStock(artesaniaId) < cantidad)
            {
                throw new InvalidOperationException(
                    "No hay suficientes unidades disponibles.");
            }

            // Buscar lotes, comenzando por los más antiguos
            var lotesDisponibles = lotes
                .Where(l => l.ArtesaniaId == artesaniaId &&
                            l.CantidadDisponible > 0)
                .OrderBy(l => l.FechaRecepcion)
                .ToList();

            int cantidadPendiente = cantidad;

            foreach (var lote in lotesDisponibles)
            {
                if (cantidadPendiente == 0)
                    break;

                int cantidadVender = Math.Min(
                    cantidadPendiente,
                    lote.CantidadDisponible);

                // Descontar unidades
                lote.Vender(cantidadVender);

                // Registrar el movimiento
                Movimiento movimiento = new Movimiento
                {
                    Id = movimientos.Count + 1,
                    ArtesaniaId = artesaniaId,
                    ArtesanoId = artesania.ArtesanoId,
                    LoteId = lote.Id,
                    Tipo = TipoMovimiento.Venta,
                    Fecha = DateTime.Now,
                    Cantidad = cantidadVender,
                    PrecioUnitario = artesania.PrecioVenta,
                    ComisionTotal =
                        artesania.ObtenerComision() * cantidadVender
                };

                movimientos.Add(movimiento);

                cantidadPendiente -= cantidadVender;
            }

            return artesania.PrecioVenta * cantidad;
        }


        public void DevolverArtesania(
            int artesaniaId,
            int artesanoId,
            int cantidad)
        {
            // Validar cantidad
            if (cantidad <= 0)
            {
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");
            }

            // Buscar artesanía
            Artesania artesania = artesanias.FirstOrDefault(
                a => a.Id == artesaniaId);

            if (artesania == null)
            {
                throw new InvalidOperationException(
                    "La artesanía no existe.");
            }

            // Verificar propietario
            if (artesania.ArtesanoId != artesanoId)
            {
                throw new InvalidOperationException(
                    "La artesanía no pertenece a este artesano.");
            }

            // Verificar stock disponible
            if (ObtenerStock(artesaniaId) < cantidad)
            {
                throw new InvalidOperationException(
                    "No hay suficientes unidades para devolver.");
            }

            // Seleccionar primero los lotes más antiguos
            var lotesDisponibles = lotes
                .Where(l => l.ArtesaniaId == artesaniaId &&
                            l.CantidadDisponible > 0)
                .OrderBy(l => l.FechaRecepcion)
                .ToList();

            int cantidadPendiente = cantidad;

            foreach (var lote in lotesDisponibles)
            {
                if (cantidadPendiente == 0)
                    break;

                int cantidadDevolver = Math.Min(
                    cantidadPendiente,
                    lote.CantidadDisponible);

                // Descontar unidades del lote
                lote.Devolver(cantidadDevolver);

                // Registrar movimiento de devolución
                Movimiento movimiento = new Movimiento
                {
                    Id = movimientos.Count + 1,
                    ArtesaniaId = artesaniaId,
                    ArtesanoId = artesanoId,
                    LoteId = lote.Id,
                    Tipo = TipoMovimiento.Devolucion,
                    Fecha = DateTime.Now,
                    Cantidad = cantidadDevolver,
                    PrecioUnitario = artesania.PrecioVenta,
                    ComisionTotal = 0m
                };

                movimientos.Add(movimiento);

                cantidadPendiente -= cantidadDevolver;
            }
        }

        public void EliminarArtesano(int artesanoId)
        {
            // Buscar al artesano
            Artesano artesano = artesanos.FirstOrDefault(
                a => a.Id == artesanoId);

            if (artesano == null)
            {
                throw new InvalidOperationException(
                    "El artesano no existe.");
            }

            // Verificar si tiene artesanías registradas
            if (artesanias.Any(a => a.ArtesanoId == artesanoId))
            {
                throw new InvalidOperationException(
                    "No se puede eliminar un artesano que tiene artesanías registradas.");
            }

            // Eliminar si no tiene artesanías
            artesanos.Remove(artesano);
        }


        public void EliminarArtesania(int artesaniaId)
        {
            // Buscar la artesanía
            Artesania artesania = artesanias.FirstOrDefault(
                a => a.Id == artesaniaId);

            if (artesania == null)
            {
                throw new InvalidOperationException(
                    "La artesanía no existe.");
            }

            // Verificar movimientos registrados
            if (movimientos.Any(m => m.ArtesaniaId == artesaniaId))
            {
                throw new InvalidOperationException(
                    "No se puede eliminar una artesanía que tiene movimientos registrados.");
            }

            // Verificar lotes relacionados
            if (lotes.Any(l => l.ArtesaniaId == artesaniaId))
            {
                throw new InvalidOperationException(
                    "No se puede eliminar una artesanía que tiene lotes registrados.");
            }

            // Eliminar artesanía
            artesanias.Remove(artesania);
        }

        public void MostrarArtesaniasParaDevolucion()
        {
            Console.WriteLine("\n=== ARTESANIAS PARA DEVOLUCION ===");

            var lotesVencidos = lotes
                .Where(l => l.DebeDevolverse())
                .OrderBy(l => l.FechaRecepcion)
                .ToList();

            if (lotesVencidos.Count == 0)
            {
                Console.WriteLine("No hay artesanías pendientes de devolución.");
                return;
            }

            foreach (var lote in lotesVencidos)
            {
                Artesania artesania = artesanias.First(
                    a => a.Id == lote.ArtesaniaId);

                Artesano artesano = artesanos.First(
                    a => a.Id == artesania.ArtesanoId);

                int dias = (DateTime.Today - lote.FechaRecepcion.Date).Days;

                Console.WriteLine("Producto: " + artesania.Nombre);
                Console.WriteLine("Artesano: " + artesano.Nombre);
                Console.WriteLine("Comunidad: " + artesano.Comunidad);
                Console.WriteLine("Lote: " + lote.Id);
                Console.WriteLine("Días en tienda: " + dias);
                Console.WriteLine("Unidades por devolver: " +
                    lote.CantidadDisponible);
                Console.WriteLine("----------------------------");
            }
        }
        // Guardar información de la tienda en JSON
        public void GuardarDatos(string ruta)
        {
            DatosTienda datos = new DatosTienda
            {
                Artesanos = artesanos,
                Artesanias = artesanias,
                Lotes = lotes,
                Movimientos = movimientos
            };

            ArchivoServicio archivo = new ArchivoServicio();

            archivo.Guardar(ruta, datos);
        }

        // Recuperar información desde JSON
        public void CargarDatos(string ruta)
        {
            ArchivoServicio archivo = new ArchivoServicio();

            DatosTienda datos = archivo.Cargar(ruta);

            // Limpiar información anterior
            artesanos.Clear();
            artesanias.Clear();
            lotes.Clear();
            movimientos.Clear();

            // Recuperar los datos
            artesanos.AddRange(datos.Artesanos);
            artesanias.AddRange(datos.Artesanias);
            lotes.AddRange(datos.Lotes);
            movimientos.AddRange(datos.Movimientos);
        }


        // cuarto consultamos el inventario
        public int ObtenerStock(int artesaniaId)
        {
            return lotes
                .Where(l => l.ArtesaniaId == artesaniaId)
                .Sum(l => l.CantidadDisponible);
        }


        public IReadOnlyList<Movimiento> ObtenerMovimientos()
        {
            return movimientos.AsReadOnly();
        }

        public IReadOnlyList<Artesano> ObtenerArtesanos()
        {
            return artesanos.AsReadOnly();
        }


        // Consultar las artesanías registradas
        public IReadOnlyList<Artesania> ObtenerArtesanias()
        {
            return artesanias.AsReadOnly();
        }

        // Consultar los lotes registrados
        public IReadOnlyList<LotesConsignacion> ObtenerLotes()
        {
            return lotes.AsReadOnly();
        }


        // consultar cantidades registradas
        public int TotalArtesanos => artesanos.Count;
        public int TotalArtesanias => artesanias.Count;
        public int TotalMovimientos => movimientos.Count;
    }
}
