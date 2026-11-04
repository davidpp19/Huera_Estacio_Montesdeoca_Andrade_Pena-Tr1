using Ejercicio_4.Datos;
using Ejercicio_4.Modelos;
using Ejercicio_4.Servicios;
using System;
using System.Collections.Generic;

namespace Ejercicio_4
{
    public class MenuPrincipal
    {
        private GestorArchivo gestorArchivos;
        private ServicioInventario servicioInventario;
        private ServicioVenta servicioVentas;

        private List<Ingrediente> listaIngredientes;
        private List<Plato> listaPlatos;
        private List<Bebida> listaBebidas;
        private List<RegistroVenta> listaVentas;
        private List<RegistroAbastecimiento> listaAbastecimientos;

        public MenuPrincipal()
        {
            gestorArchivos = new GestorArchivo();
            servicioInventario = new ServicioInventario();
            servicioVentas = new ServicioVenta();


            listaIngredientes = gestorArchivos.CargarIngredientes();
            listaPlatos = gestorArchivos.CargarPlatos();
            listaBebidas = gestorArchivos.CargarBebidas();
            listaVentas = gestorArchivos.CargarVentas();
            listaAbastecimientos = gestorArchivos.CargarAbastecimientos();
        }

        public void Iniciar()
        {
            bool salir = false;
            while (salir == false)
            {
                Console.Clear();
                Console.WriteLine("CAFETERIA UTN: ");
                Console.WriteLine("1. Vender Plato");
                Console.WriteLine("2. Reporte: Costo, precio y margen de cada plato");
                Console.WriteLine("3. Reporte: Lista de compras (Stock mínimo)");
                Console.WriteLine("4. Reporte: Platos que se pueden preparar");
                Console.WriteLine("5. Reporte: Platos mas vendidos en un periodo");
                Console.WriteLine("6. Reporte: Auditoria del stock");
                Console.WriteLine("7. Guardar y Salir");
                Console.Write("Seleccione una opcion: ");

                string opcion = Console.ReadLine();

                if (opcion == "1") { RealizarVentaPrueba(); }
                else if (opcion == "2") { ReporteCostoPrecioMargen(); }
                else if (opcion == "3") { ReporteListaCompras(); }
                else if (opcion == "4") { ReportePlatosPosibles(); }
                else if (opcion == "5") { ReporteMasVendidos(); }
                else if (opcion == "6") { ReporteAuditoria(); }
                else if (opcion == "7")
                {
                    GuardarTodo();
                    salir = true;
                }

                if (salir == false)
                {
                    Console.WriteLine("\nPresione ENTER para continuar...");
                    Console.ReadLine();
                }
            }
        }

        private void RealizarVentaPrueba()
        {
            if (listaPlatos.Count == 0)
            {
                Console.WriteLine("No hay platos registrados. (Debes crear datos de prueba primero).");
                return;
            }

            Plato platoElegido = listaPlatos[0]; 
            Console.Write($"Cuantos '{platoElegido.nombrePlato}' desea vender?: ");
            string entrada = Console.ReadLine();
            int cantidad = int.Parse(entrada);

            bool exito = servicioVentas.VenderPlato(platoElegido, cantidad, listaVentas);

            if (exito == true)
            {
                Console.WriteLine("Venta exitosa. Se descontaron los ingredientes.");
            }
            else
            {
                Console.WriteLine("Venta rechazada. Falta stock de algun ingrediente en la receta.");
            }
        }
        private void ReporteCostoPrecioMargen()
        {
            Console.WriteLine("\n COSTO, PRECIO Y MARGEN DE CADA PLATO");
            foreach (Plato plato in listaPlatos)
            {
                Console.WriteLine($"Plato: {plato.nombrePlato}");
                Console.WriteLine($"Costo: ${plato.CalcularCosto()}");
                Console.WriteLine($"Precio de Venta: ${plato.CalcularPrecioVenta()}");
                Console.WriteLine($"Margen de Ganancia: ${plato.CalcularMargen()}\n");
            }
        }

        private void ReporteListaCompras()
        {
            Console.WriteLine("\n LISTA DE COMPRAS (POR DEBAJO DEL STOCK MINIMO)");
            List<Ingrediente> paraComprar = servicioInventario.ObtenerListaCompras(listaIngredientes);

            if (paraComprar.Count == 0)
            {
                Console.WriteLine("Todos los ingredientes tienen buen stock.");
            }
            else
            {
                foreach (Ingrediente ing in paraComprar)
                {
                    Console.WriteLine($"- {ing.nombreIngrediente}: Stock actual = {ing.stockActual}, Minimo requerido = {ing.stockMinimo}");
                }
            }
        }

        private void ReportePlatosPosibles()
        {
            Console.WriteLine("\n PLATOS QUE SE PUEDEN PREPARAR CON EL STOCK ACTUAL: ");
            foreach (Plato plato in listaPlatos)
            {
                int cantidad = servicioInventario.CalcularPlatosPosibles(plato);
                Console.WriteLine($"Del plato '{plato.nombrePlato}' se pueden preparar {cantidad} unidades.");
            }
        }

        private void ReporteMasVendidos()
        {
            Console.WriteLine("\n PLATOS MAS VENDIDOS EN UN PERIODO");
            DateTime fechaInicio = DateTime.Now.AddDays(-30);
            DateTime fechaFin = DateTime.Now;

            List<string> nombresProductos = new List<string>();
            List<int> cantidadesVendidas = new List<int>();

            foreach (RegistroVenta venta in listaVentas)
            {
                if (venta.fechaVenta >= fechaInicio && venta.fechaVenta <= fechaFin)
                {
                    int indice = nombresProductos.IndexOf(venta.nombreProducto);

                    if (indice == -1) 
                    {
                        nombresProductos.Add(venta.nombreProducto);
                        cantidadesVendidas.Add(venta.cantidadVendida);
                    }
                    else 
                    {
                        cantidadesVendidas[indice] = cantidadesVendidas[indice] + venta.cantidadVendida;
                    }
                }
            }

            for (int i = 0; i < nombresProductos.Count; i++)
            {
                Console.WriteLine($"- {nombresProductos[i]}: {cantidadesVendidas[i]} unidades vendidas.");
            }
        }

        private void ReporteAuditoria()
        {
            Console.WriteLine("\n AUDITORIA DEL STOCK DE INGREDIENTES");
            foreach (Ingrediente ing in listaIngredientes)
            {
                Console.WriteLine($"Ingrediente: {ing.nombreIngrediente} | Stock en sistema: {ing.stockActual} {ing.unidadMedida}");
            }
        }

        private void GuardarTodo()
        {
            gestorArchivos.GuardarIngredientes(listaIngredientes);
            gestorArchivos.GuardarPlatos(listaPlatos);
            gestorArchivos.GuardarBebidas(listaBebidas);
            gestorArchivos.GuardarVentas(listaVentas);
            gestorArchivos.GuardarAbastecimientos(listaAbastecimientos);
            Console.WriteLine("Informacion guardada en archivos exitosamente.");
        }
    }
}
