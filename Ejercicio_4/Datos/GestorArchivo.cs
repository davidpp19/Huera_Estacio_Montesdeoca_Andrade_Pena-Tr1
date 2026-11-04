using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Ejercicio_4.Modelos;

namespace Ejercicio_4.Datos
{
    public class GestorArchivo
    {
        public void GuardarIngredientes(List<Ingrediente> listaIngredientes)
        {
            string texto = JsonSerializer.Serialize(listaIngredientes);
            File.WriteAllText("ingredientes.json", texto);
        }
        public List<Ingrediente> CargarIngredientes()
        {
            if (File.Exists("ingredientes.json"))
            {
                string texto = File.ReadAllText("ingredientes.json");
                return JsonSerializer.Deserialize<List<Ingrediente>>(texto);
            }
            return new List<Ingrediente>();
        }
        public void GuardarPlatos(List<Plato> listaPlatos)
        {
            string texto = JsonSerializer.Serialize(listaPlatos);
            File.WriteAllText("platos.json", texto);
        }
        public List<Plato> CargarPlatos()
        {
            if (File.Exists("platos.json"))
            {
                string texto = File.ReadAllText("platos.json");
                return JsonSerializer.Deserialize<List<Plato>>(texto);
            }
            return new List<Plato>();
        }
        public void GuardarBebidas(List<Bebida> listaBebidas)
        {
            string texto = JsonSerializer.Serialize(listaBebidas);
            File.WriteAllText("bebidas.json", texto);
        }

        public List<Bebida> CargarBebidas()
        {
            if (File.Exists("bebidas.json"))
            {
                string texto = File.ReadAllText("bebidas.json");
                return JsonSerializer.Deserialize<List<Bebida>>(texto);
            }
            return new List<Bebida>();
        }
        public void GuardarVentas(List<RegistroVenta> listaVentas)
        {
            string texto = JsonSerializer.Serialize(listaVentas);
            File.WriteAllText("ventas.json", texto);
        }

        public List<RegistroVenta> CargarVentas()
        {
            if (File.Exists("ventas.json"))
            {
                string texto = File.ReadAllText("ventas.json");
                return JsonSerializer.Deserialize<List<RegistroVenta>>(texto);
            }
            return new List<RegistroVenta>();
        }
        public void GuardarAbastecimientos(List<RegistroAbastecimiento> listaAbastecimientos)
        {
            string texto = JsonSerializer.Serialize(listaAbastecimientos);
            File.WriteAllText("abastecimientos.json", texto);
        }
        public List<RegistroAbastecimiento> CargarAbastecimientos()
        {
            if (File.Exists("abastecimientos.json"))
            {
                string texto = File.ReadAllText("abastecimientos.json");
                return JsonSerializer.Deserialize<List<RegistroAbastecimiento>>(texto);
            }
            return new List<RegistroAbastecimiento>();
        }
    }
}
