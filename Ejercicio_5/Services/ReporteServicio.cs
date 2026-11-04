
using System;
using System.Linq;
using Ejercicio_5.Models;

namespace Ejercicio_5.Servicios
{
    public class ReporteServicio
    {
        private readonly TiendaServicio tienda;

        public ReporteServicio(TiendaServicio tienda)
        {
            this.tienda = tienda;
        }

        // Reporte 1: Liquidacion de artesanos
        public void MostrarLiquidacion(
            int artesanoId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                throw new ArgumentException(
                    "El periodo de fechas no es valido.");
            }

            Artesano artesano = tienda.ObtenerArtesanos()
                .FirstOrDefault(a => a.Id == artesanoId);

            if (artesano == null)
            {
                throw new InvalidOperationException(
                    "El artesano no existe.");
            }

            var ventas = tienda.ObtenerMovimientos()
                .Where(m => m.ArtesanoId == artesanoId
                         && m.Tipo == TipoMovimiento.Venta
                         && m.Fecha.Date >= fechaInicio.Date
                         && m.Fecha.Date <= fechaFin.Date)
                .ToList();

            decimal totalVendido =
                ventas.Sum(m => m.ObtenerTotalVenta());

            decimal totalComision =
                ventas.Sum(m => m.ComisionTotal);

            decimal pagoArtesano =
                totalVendido - totalComision;

            Console.WriteLine("\n=== LIQUIDACION DE ARTESANO ===");
            Console.WriteLine("Artesano: " + artesano.Nombre);
            Console.WriteLine("Comunidad: " + artesano.Comunidad);
            Console.WriteLine("Desde: " +
                fechaInicio.ToString("dd/MM/yyyy"));
            Console.WriteLine("Hasta: " +
                fechaFin.ToString("dd/MM/yyyy"));

            Console.WriteLine("Unidades vendidas: " +
                ventas.Sum(m => m.Cantidad));

            Console.WriteLine("Total vendido: $" +
                totalVendido.ToString("F2"));

            Console.WriteLine("Comision tienda: $" +
                totalComision.ToString("F2"));

            Console.WriteLine("Pago al artesano: $" +
                pagoArtesano.ToString("F2"));
        }


        // Reporte 2: Artesanías pendientes de devolución
        public void MostrarPendientesDevolucion()
        {
            tienda.MostrarArtesaniasParaDevolucion();
        }


        // Reporte 3: Comunidades con más ventas
        public void MostrarComunidadesConMasVentas(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                throw new ArgumentException(
                    "El periodo de fechas no es valido.");
            }

            // Consultar ventas y relacionarlas con artesanos
            var ventasPorComunidad =
                from movimiento in tienda.ObtenerMovimientos()
                join artesano in tienda.ObtenerArtesanos()
                    on movimiento.ArtesanoId equals artesano.Id

                where movimiento.Tipo == TipoMovimiento.Venta
                   && movimiento.Fecha.Date >= fechaInicio.Date
                   && movimiento.Fecha.Date <= fechaFin.Date

                group movimiento by artesano.Comunidad into grupo

                select new
                {
                    Comunidad = grupo.Key,

                    UnidadesVendidas =
                        grupo.Sum(m => m.Cantidad),

                    TotalVendido =
                        grupo.Sum(m => m.ObtenerTotalVenta())
                };

            // Ordenar de mayor a menor venta
            var resultados = ventasPorComunidad
                .OrderByDescending(c => c.TotalVendido)
                .ThenByDescending(c => c.UnidadesVendidas)
                .ThenBy(c => c.Comunidad)
                .ToList();

            Console.WriteLine(
                "\n=== COMUNIDADES CON MAS VENTAS ===");

            Console.WriteLine("Desde: " +
                fechaInicio.ToString("dd/MM/yyyy"));

            Console.WriteLine("Hasta: " +
                fechaFin.ToString("dd/MM/yyyy"));

            if (resultados.Count == 0)
            {
                Console.WriteLine(
                    "No existen ventas en este periodo.");
                return;
            }

            int posicion = 1;

            foreach (var comunidad in resultados)
            {
                Console.WriteLine(
                    "\nPosicion: " + posicion);

                Console.WriteLine(
                    "Comunidad: " + comunidad.Comunidad);

                Console.WriteLine(
                    "Unidades vendidas: " +
                    comunidad.UnidadesVendidas);

                Console.WriteLine(
                    "Total vendido: $" +
                    comunidad.TotalVendido.ToString("F2"));

                posicion++;
            }
        }

        // Reporte 4: Artesanías más vendidas
        public void MostrarArtesaniasMasVendidas(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                throw new ArgumentException(
                    "El periodo de fechas no es valido.");
            }

            // Relacionar las ventas con las artesanías
            var ventas =
                from movimiento in tienda.ObtenerMovimientos()
                join artesania in tienda.ObtenerArtesanias()
                    on movimiento.ArtesaniaId equals artesania.Id

                where movimiento.Tipo == TipoMovimiento.Venta
                   && movimiento.Fecha.Date >= fechaInicio.Date
                   && movimiento.Fecha.Date <= fechaFin.Date

                group movimiento by new
                {
                    artesania.Id,
                    artesania.Nombre
                }
                into grupo

                select new
                {
                    Producto = grupo.Key.Nombre,
                    CantidadVendida =
                        grupo.Sum(m => m.Cantidad),
                    TotalVendido =
                        grupo.Sum(m => m.ObtenerTotalVenta())
                };

            // Ordenar de mayor a menor cantidad vendida
            var resultados = ventas
                .OrderByDescending(v => v.CantidadVendida)
                .ThenByDescending(v => v.TotalVendido)
                .ThenBy(v => v.Producto)
                .ToList();

            Console.WriteLine(
                "\n=== ARTESANIAS MAS VENDIDAS ===");

            Console.WriteLine("Desde: " +
                fechaInicio.ToString("dd/MM/yyyy"));

            Console.WriteLine("Hasta: " +
                fechaFin.ToString("dd/MM/yyyy"));

            if (resultados.Count == 0)
            {
                Console.WriteLine(
                    "No existen ventas en este periodo.");
                return;
            }

            int posicion = 1;

            foreach (var producto in resultados)
            {
                Console.WriteLine(
                    "\nPosicion: " + posicion);

                Console.WriteLine(
                    "Artesania: " + producto.Producto);

                Console.WriteLine(
                    "Unidades vendidas: " +
                    producto.CantidadVendida);

                Console.WriteLine(
                    "Total vendido: $" +
                    producto.TotalVendido.ToString("F2"));

                posicion++;
            }
        }

        // Reporte 5: Auditoría del inventario
        public void MostrarAuditoriaInventario()
        {
            Console.WriteLine(
                "\n=== AUDITORIA DE INVENTARIO ===");

            var artesanias = tienda.ObtenerArtesanias();
            var lotes = tienda.ObtenerLotes();
            var movimientos = tienda.ObtenerMovimientos();

            if (artesanias.Count == 0)
            {
                Console.WriteLine(
                    "No existen artesanías registradas.");
                return;
            }

            int totalRecibido = 0;
            int totalVendido = 0;
            int totalDevuelto = 0;
            int totalDisponible = 0;

            bool auditoriaCorrecta = true;

            foreach (var artesania in artesanias)
            {
                // Obtener lotes de la artesanía
                var lotesProducto = lotes
                    .Where(l => l.ArtesaniaId == artesania.Id)
                    .ToList();

                // Obtener movimientos de la artesanía
                var movimientosProducto = movimientos
                    .Where(m => m.ArtesaniaId == artesania.Id)
                    .ToList();

                // Calcular cantidades a partir de movimientos
                int recibidas = movimientosProducto
                    .Where(m => m.Tipo == TipoMovimiento.Recepcion)
                    .Sum(m => m.Cantidad);

                int vendidas = movimientosProducto
                    .Where(m => m.Tipo == TipoMovimiento.Venta)
                    .Sum(m => m.Cantidad);

                int devueltas = movimientosProducto
                    .Where(m => m.Tipo == TipoMovimiento.Devolucion)
                    .Sum(m => m.Cantidad);

                // Cantidad que debería estar disponible
                int stockEsperado =
                    recibidas - vendidas - devueltas;

                // Cantidad realmente registrada en lotes
                int stockActual = lotesProducto
                    .Sum(l => l.CantidadDisponible);

                // Comprobar cada lote contra sus movimientos
                bool lotesCorrectos = lotesProducto.All(l =>
                {
                    var movimientosLote = movimientosProducto
                        .Where(m => m.LoteId == l.Id)
                        .ToList();

                    int r = movimientosLote
                        .Where(m => m.Tipo == TipoMovimiento.Recepcion)
                        .Sum(m => m.Cantidad);

                    int v = movimientosLote
                        .Where(m => m.Tipo == TipoMovimiento.Venta)
                        .Sum(m => m.Cantidad);

                    int d = movimientosLote
                        .Where(m => m.Tipo == TipoMovimiento.Devolucion)
                        .Sum(m => m.Cantidad);

                    return l.CantidadRecibida == r
                        && l.CantidadVendida == v
                        && l.CantidadDevuelta == d
                        && l.CantidadDisponible >= 0
                        && l.CantidadVendida >= 0
                        && l.CantidadDevuelta >= 0
                        && l.VerificarInventario();
                });

                // Verificar que todos los movimientos tengan lote
                bool referenciasCorrectas =
                    movimientosProducto.All(m =>
                        lotesProducto.Any(l => l.Id == m.LoteId));

                bool correcto =
                    stockEsperado == stockActual
                    && stockEsperado >= 0
                    && lotesCorrectos
                    && referenciasCorrectas;

                Console.WriteLine(
                    "\nArtesania: " + artesania.Nombre);

                Console.WriteLine(
                    "Unidades recibidas: " + recibidas);

                Console.WriteLine(
                    "Unidades vendidas: " + vendidas);

                Console.WriteLine(
                    "Unidades devueltas: " + devueltas);

                Console.WriteLine(
                    "Stock esperado: " + stockEsperado);

                Console.WriteLine(
                    "Stock registrado: " + stockActual);

                Console.WriteLine(
                    "Estado: " +
                    (correcto ? "CORRECTO" : "ERROR"));

                Console.WriteLine("--------------------------");

                if (!correcto)
                {
                    auditoriaCorrecta = false;
                }

                totalRecibido += recibidas;
                totalVendido += vendidas;
                totalDevuelto += devueltas;
                totalDisponible += stockActual;
            }

            // Resumen general
            Console.WriteLine("\n=== RESUMEN DEL INVENTARIO ===");

            Console.WriteLine(
                "Total recibido: " + totalRecibido);

            Console.WriteLine(
                "Total vendido: " + totalVendido);

            Console.WriteLine(
                "Total devuelto: " + totalDevuelto);

            Console.WriteLine(
                "Total disponible: " + totalDisponible);

            Console.WriteLine(
                "Estado general: " +
                (auditoriaCorrecta ? "CORRECTO" : "REVISAR"));
        }
    }
}
