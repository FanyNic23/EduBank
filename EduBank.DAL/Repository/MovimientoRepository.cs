using EduBank.DAL.DataContext;
using EduBank.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.DAL.Repository
{
    public class MovimientoRepository : IMovimientoRepository
    {
        private readonly EdubanckssrContext _db;
        public MovimientoRepository(EdubanckssrContext context) { _db = context; }

        // QUERY BASE CON FILTRO POR USUARIO (a través de Cuenta)
        private IQueryable<Movimiento> QueryPorUsuario(int usuarioId)
        {
            return _db.Movimientos
                .Include(m => m.Categoria)
                .Include(m => m.Cuenta)
                .Where(m => m.Cuenta.UsuarioId == usuarioId); // ← FILTRO POR USUARIO VÍA CUENTA
        }

        // MÉTODOS DEL IGenericRepository (para operaciones internas)
        public async Task<bool> Insertar(Movimiento modelo)
        {
            await _db.Movimientos.AddAsync(modelo);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Actualizar(Movimiento modelo)
        {
            // VERIFICAR que el movimiento pertenezca al usuario
            var existe = await QueryPorUsuario(modelo.Cuenta.UsuarioId)
                .AnyAsync(m => m.MovimientoId == modelo.MovimientoId);
            if (!existe) return false;

            modelo.ActualizadoEn = DateTime.Now;
            _db.Movimientos.Update(modelo);
            return await _db.SaveChangesAsync() > 0;
        }

        // MÉTODO SEGURO PARA ELIMINAR
        public async Task<bool> EliminarPorUsuario(long id, int usuarioId)
        {
            var entidad = await QueryPorUsuario(usuarioId)
                .FirstOrDefaultAsync(m => m.MovimientoId == id);
            if (entidad == null) return false;

            _db.Movimientos.Remove(entidad);
            return await _db.SaveChangesAsync() > 0;
        }

        // MÉTODO DEL IGenericRepository (marcar como obsoleto)
        public async Task<bool> Eliminar(int id)
        {
            throw new InvalidOperationException("Usar EliminarPorUsuario en su lugar");
        }

        // MÉTODO SEGURO PARA OBTENER
        public async Task<Movimiento?> ObtenerPorUsuario(long id, int usuarioId)
        {
            return await QueryPorUsuario(usuarioId)
                .FirstOrDefaultAsync(m => m.MovimientoId == id);
        }

        // MÉTODO DEL IGenericRepository (marcar como obsoleto)
        public async Task<Movimiento?> Obtener(int id)
        {
            throw new InvalidOperationException("Usar ObtenerPorUsuario en su lugar");
        }

        // MÉTODO SEGURO PARA LISTAR
        public async Task<List<Movimiento>> ObtenerPorUsuario(int usuarioId)
        {
            return await QueryPorUsuario(usuarioId)
                .OrderByDescending(m => m.FechaOperacion)
                .ToListAsync();
        }

        // MÉTODO DEL IGenericRepository (marcar como obsoleto)
        public async Task<List<Movimiento>> ObtenerTodos()
        {
            throw new InvalidOperationException("Usar ObtenerPorUsuario en su lugar");
        }

        // MÉTODOS ESPECÍFICOS ACTUALIZADOS
        public async Task<decimal> ObtenerTotalPorTipo(char tipo, int usuarioId)
        {
            return await QueryPorUsuario(usuarioId)
                .Where(m => m.Tipo == tipo.ToString())
                .SumAsync(m => (decimal?)m.Monto) ?? 0m;
        }

        public async Task<IEnumerable<(int CategoriaId, string CategoriaNombre, decimal Total)>> ObtenerTotalesPorCategoria(int usuarioId, char? tipo = null)
        {
            var q = QueryPorUsuario(usuarioId);

            if (tipo.HasValue)
                q = q.Where(m => m.Tipo == tipo.Value.ToString());

            var grouped = await q
                .GroupBy(m => new { m.CategoriaId, m.Categoria.Nombre })
                .Select(g => new
                {
                    CategoriaId = g.Key.CategoriaId,
                    CategoriaNombre = g.Key.Nombre,
                    Total = g.Sum(x => x.Monto)
                }).ToListAsync();

            return grouped.Select(x => (x.CategoriaId, x.CategoriaNombre, x.Total));
        }

        public async Task<IEnumerable<Movimiento>> ObtenerRecientes(int usuarioId, int top = 10)
        {
            return await QueryPorUsuario(usuarioId)
                .OrderByDescending(m => m.FechaOperacion)
                .ThenByDescending(m => m.CreadoEn)
                .Take(top)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movimiento>> ObtenerPorRango(int usuarioId, DateTime desde, DateTime hasta)
        {
            return await QueryPorUsuario(usuarioId)
                .Where(m => m.FechaOperacion >= desde && m.FechaOperacion <= hasta)
                .OrderByDescending(m => m.FechaOperacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movimiento>> ObtenerPorPeriodo(int usuarioId, string periodo, DateTime fechaReferencia)
        {
            var query = QueryPorUsuario(usuarioId);
            var (fechaInicio, fechaFin) = CalcularRangoFechas(periodo, fechaReferencia);

            return await query
                .Where(m => m.FechaOperacion >= fechaInicio && m.FechaOperacion <= fechaFin)
                .OrderByDescending(m => m.FechaOperacion)
                .ToListAsync();
        }

        public async Task<(DateTime Inicio, DateTime Fin)> ObtenerRangoPeriodo(string periodo, DateTime fechaReferencia)
        {
            return await Task.FromResult(CalcularRangoFechas(periodo, fechaReferencia));
        }
        private (DateTime Inicio, DateTime Fin) CalcularRangoFechas(string periodo, DateTime fechaReferencia)
        {
            DateTime inicio, fin;

            switch (periodo.ToLower())
            {
                case "dia":
                    inicio = fechaReferencia.Date;
                    fin = fechaReferencia.Date.AddDays(1).AddTicks(-1);
                    break;

                case "semana":
                    // Asumiendo que la semana comienza el lunes
                    int diff = (7 + (fechaReferencia.DayOfWeek - DayOfWeek.Monday)) % 7;
                    inicio = fechaReferencia.AddDays(-1 * diff).Date;
                    fin = inicio.AddDays(7).AddTicks(-1);
                    break;

                case "mes":
                    inicio = new DateTime(fechaReferencia.Year, fechaReferencia.Month, 1);
                    fin = inicio.AddMonths(1).AddTicks(-1);
                    break;

                case "año":
                    inicio = new DateTime(fechaReferencia.Year, 1, 1);
                    fin = new DateTime(fechaReferencia.Year + 1, 1, 1).AddTicks(-1);
                    break;

                default:
                    throw new ArgumentException("Período no válido");
            }

            return (inicio, fin);
        }



    
    }

}