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
                Console.WriteLine("1. Agregar Ingrediente nuevo");
                Console.WriteLine("2. Agregar Plato nuevo (y armar receta)");
                Console.WriteLine("3. Vender Plato");
                Console.WriteLine("4. Reporte: Costo, precio y margen de cada plato");
                Console.WriteLine("5. Reporte: Lista de compras (Stock mínimo)");
                Console.WriteLine("6. Reporte: Platos que se pueden preparar");
                Console.WriteLine("7. Reporte: Platos mas vendidos en un periodo");
                Console.WriteLine("8. Reporte: Auditoria del stock");
                Console.WriteLine("9. Eliminar Ingrediente ");
                Console.WriteLine("10. Guardar y Salir");
                Console.Write("Seleccione una opcion: ");

                string opcion = Console.ReadLine();

                if (opcion == "1") { AgregarIngrediente(); }
                else if (opcion == "2") { AgregarPlato(); }
                else if (opcion == "3") { RealizarVentaPrueba(); }
                else if (opcion == "4") { ReporteCostoPrecioMargen(); }
                else if (opcion == "5") { ReporteListaCompras(); }
                else if (opcion == "6") { ReportePlatosPosibles(); }
                else if (opcion == "7") { ReporteMasVendidos(); }
                else if (opcion == "8") { ReporteAuditoria(); }
                else if (opcion == "9") { EliminarIngredientePrueba(); }
                else if (opcion == "10")
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
                Console.WriteLine("No hay platos registrados.");
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
            Console.WriteLine("\nCOSTO, PRECIO Y MARGEN DE CADA PLATO: ");
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
            Console.WriteLine("\nLISTA DE COMPRAS (POR DEBAJO DEL STOCK MINIMO): ");
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
            Console.WriteLine("\nPLATOS QUE SE PUEDEN PREPARAR CON EL STOCK ACTUAL: ");
            foreach (Plato plato in listaPlatos)
            {
                int cantidad = servicioInventario.CalcularPlatosPosibles(plato);
                Console.WriteLine($"Del plato '{plato.nombrePlato}' se pueden preparar {cantidad} unidades.");
            }
        }

        private void ReporteMasVendidos()
        {
            Console.WriteLine("\nPLATOS MAS VENDIDOS EN UN PERIODO:");
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
            Console.WriteLine("\nAUDITORIA DEL STOCK DE INGREDIENTES: ");
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
        private void EliminarIngredientePrueba()
        {
            Console.WriteLine("\nELIMINAR INGREDIENTE: ");
            if (listaIngredientes.Count == 0)
            {
                Console.WriteLine("No hay ingredientes registrados.");
                return;
            }

            for (int i = 0; i < listaIngredientes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {listaIngredientes[i].nombreIngrediente}");
            }

            Console.Write("Seleccione el numero del ingrediente a eliminar: ");
            string entrada = Console.ReadLine();
            int indice = int.Parse(entrada) - 1;

            Ingrediente ingElegido = listaIngredientes[indice];

            if (servicioInventario.PuedeEliminarIngrediente(ingElegido) == true)
            {
                listaIngredientes.Remove(ingElegido);
                Console.WriteLine($"El ingrediente '{ingElegido.nombreIngrediente}' ha sido eliminado exitosamente.");
            }
            else
            {
                Console.WriteLine("Accion rechazada: No se puede eliminar, el ingrediente esta en uso en una receta");
            }
        }
        private void AgregarIngrediente()
        {
            Console.WriteLine("\nAGREGAR NUEVO INGREDIENTE: ");
            Ingrediente nuevoIngrediente = new Ingrediente();

            Console.Write("Nombre del ingrediente: ");
            nuevoIngrediente.nombreIngrediente = Console.ReadLine();

            Console.Write("Unidad de medida (ej. kg, litros, unidades): ");
            nuevoIngrediente.unidadMedida = Console.ReadLine();

            Console.Write("Stock actual: ");
            nuevoIngrediente.stockActual = double.Parse(Console.ReadLine());

            Console.Write("Stock minimo: ");
            nuevoIngrediente.stockMinimo = double.Parse(Console.ReadLine());

            Console.Write("Costo por unidad: ");
            nuevoIngrediente.costoUnidad = double.Parse(Console.ReadLine());

            listaIngredientes.Add(nuevoIngrediente);
            Console.WriteLine($"Ingrediente '{nuevoIngrediente.nombreIngrediente}' agregado correctamente.");
        }

        private void AgregarPlato()
        {
            Console.WriteLine("\nAGREGAR NUEVO PLATO: ");
            if (listaIngredientes.Count == 0)
            {
                Console.WriteLine("Primero debe registrar ingredientes para poder armar una receta.");
                return;
            }

            Plato nuevoPlato = new Plato();

            Console.Write("Nombre del plato: ");
            nuevoPlato.nombrePlato = Console.ReadLine();

            bool agregandoIngredientes = true;

            while (agregandoIngredientes == true)
            {
                Console.WriteLine("\nIngredientes disponibles: ");
                for (int i = 0; i < listaIngredientes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {listaIngredientes[i].nombreIngrediente}");
                }

                Console.Write("Seleccione el numero del ingrediente a agregar a la receta (o '0' para terminar): ");
                string seleccion = Console.ReadLine();
                int indice = int.Parse(seleccion) - 1;

                if (seleccion == "0")
                {
                    agregandoIngredientes = false;
                }
                else if (indice >= 0 && indice < listaIngredientes.Count)
                {
                    Ingrediente ingElegido = listaIngredientes[indice];

                    IngredienteReceta nuevoItem = new IngredienteReceta();
                    nuevoItem.ingredienteBase = ingElegido;

                    Console.Write($"Que cantidad de '{ingElegido.nombreIngrediente}' se usa en este plato?: ");
                    nuevoItem.cantidadUsada = double.Parse(Console.ReadLine());

                    nuevoPlato.receta.Add(nuevoItem);

                    // Marcamos el ingrediente para que no pueda ser eliminado
                    ingElegido.formaParteDeReceta = true;

                    Console.WriteLine("Ingrediente agregado a la receta.");
                }
                else
                {
                    Console.WriteLine("Opcion invalida.");
                }
            }

            if (nuevoPlato.receta.Count > 0)
            {
                listaPlatos.Add(nuevoPlato);
                Console.WriteLine($"Plato '{nuevoPlato.nombrePlato}' guardado con su receta.");
            }
            else
            {
                Console.WriteLine("No se guardo el plato porque no se agregaron ingredientes a su receta.");
            }
        }
    }
}
