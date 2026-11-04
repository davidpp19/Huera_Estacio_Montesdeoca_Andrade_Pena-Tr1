using Ejercicio_1.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_1.Servicios
{
    internal class Biblioteca
    {
        public List<Material> materiales { get; private set; }
        public List<Usuario> usuarios { get; private set; }
        public List<Prestamo> prestamos { get; private set; }
        public Biblioteca() {
            this.materiales = new List<Material>();
            this.usuarios = new List<Usuario>();
            this.prestamos = new List<Prestamo>();
        }
        public void agregarMaterial(Material material) {
            materiales.Add(material);
        }
        public void agregarUsuario(Usuario usuario) {
            usuarios.Add(usuario);
        }
        public Usuario buscarUsuario(String id) {
            foreach (Usuario u in usuarios) {
                if (u.id.Equals(id)) {
                    return u;
                }
            }
            return null;
        }
        public Material buscarMaterial(String codigo) {
            foreach (Material m in materiales) {
                if (m.codigo.Equals(codigo)) {
                    return m;
                }
            }
            return null;
        }
        public Prestamo buscarPrestamo(String id) {
            foreach (Prestamo p in prestamos) {
                if (p.id.Equals(id)) {
                    return p;
                }
            }
            return null;
        }
        public void prestar(String idUsuario, String codigoMaterial, DateTime fecha) {
            Usuario u = buscarUsuario(idUsuario);
            Material m = buscarMaterial(codigoMaterial);
            if (u == null) {
                throw new InvalidOperationException("El usuario no existe");
            }
            if (m == null) {
                throw new InvalidOperationException("El material no existe");
            }
            if (u.tieneMultasPendientes()) {
                throw new InvalidOperationException("El usuario tiene multas pendientes");
            }
            if (u.prestamosActivos() >= 3) {
                throw new InvalidOperationException("El usuario tiene ya el maximo de prestamos");
            }
            if (m.ejemplaresDisponibles == 0) {
                throw new InvalidOperationException("No hay ejemplares disponibles");
            }
            String id = "P" + (prestamos.Count + 1);
            Prestamo p = new Prestamo(id,m,u,fecha);
            m.prestar();
            this.prestamos.Add(p);
            u.prestamo.Add(p);
        }
        public void devolver(string idPrestamo, DateTime fecha) { 
            Prestamo p=buscarPrestamo(idPrestamo);
            if (p == null) {
                throw new InvalidOperationException("El prestamo no existe");
            }
            p.registrarDevolucion(fecha);
            p.material.devolver();
        }
        public void pagarMulta(String idPrestamo) {
            Prestamo p = buscarPrestamo(idPrestamo);
            if(p==null)
            {
                throw new InvalidOperationException("El prestamo no existe");
            }
            p.pagarMulta();
            
        }
        public List<Prestamo> prestamosVencidos(DateTime hoy)
        {
            List<Prestamo> vencidos = new List<Prestamo>();   
            foreach (Prestamo p in prestamos)                 
            {
                if (!p.estaDevuelto() && hoy > p.fechaVencimiento)   
                {
                    vencidos.Add(p);                          
                }
            }
            return vencidos;                                   
        }
        public List<Prestamo> historialUsuario(String idUsuario)         
        {
            List<Prestamo> historial = new List<Prestamo>();              
            foreach (Prestamo p in prestamos)
            {
                if (p.usuario.id.Equals(idUsuario))                      
                {
                    historial.Add(p);
                }
            }
            return historial;
        }
        public decimal totalMultas(DateTime desde, DateTime hasta)
        {
            decimal total = 0;
            foreach (Prestamo p in prestamos)
            {
                if (p.estaDevuelto() && p.fechaDevolucion >= desde && p.fechaDevolucion <= hasta)
                {
                    total += p.multa;
                }
            }
            return total;
        }
        public int contarPrestamos(Material m)
        {
            int contador = 0;
            foreach (Prestamo p in prestamos)
            {
                if (p.material == m)
                {
                    contador++;
                }
            }
            return contador;
        }
        public int contarActivos(Material m)
        {
            int contador = 0;
            foreach (Prestamo p in prestamos)
            {
                if (p.material == m && !p.estaDevuelto())
                {
                    contador++;
                }
            }
            return contador;
        }
        public List<Material> materialesMasPrestados(int cantidad)
        {
            return materiales
                .OrderByDescending(m => contarPrestamos(m))
                .ThenBy(m => m.codigo)
                .Take(cantidad)
                .ToList();
        }
        public bool auditoriaCorrecta(Material m)
        {
            int esperado = m.ejemplaresTotales - contarActivos(m);
            return m.ejemplaresDisponibles == esperado;
        }

    }
}
