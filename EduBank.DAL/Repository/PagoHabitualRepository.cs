// EduBank.DAL/Repository/PagoHabitualRepository.cs
using EduBank.DAL.DataContext;
using EduBank.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduBank.DAL.Repository
{
    public class PagoHabitualRepository : IPagoHabitualRepository
    {
        private readonly EdubanckssrContext _db;

        public PagoHabitualRepository(EdubanckssrContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<PagosHabituales>> ObtenerPorUsuario(int usuarioId)
        {
            return await _db.PagosHabituales
                .Include(p => p.Cuenta)
                .Include(p => p.Categoria)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<PagosHabituales>> ObtenerPagosParaEjecutar(DateTime fechaReferencia)
        {
            var hoy = fechaReferencia.Date;

            return await _db.PagosHabituales
                .Include(p => p.Cuenta)
                .Include(p => p.Categoria)
                .Where(p => p.EsActivo &&
                           p.AgregarAutomaticamente &&
                           p.FechaInicio <= hoy &&
                           (!p.FechaFin.HasValue || p.FechaFin.Value >= hoy))
                .ToListAsync();
        }

        public async Task<IEnumerable<PagosHabituales>> ObtenerProximosPagos(int usuarioId, int top = 5)
        {
            var hoy = DateTime.Today;

            var pagos = await _db.PagosHabituales
                .Include(p => p.Cuenta)
                .Include(p => p.Categoria)
                .Where(p => p.UsuarioId == usuarioId &&
                           p.EsActivo &&
                           p.FechaInicio >= hoy)
                .OrderBy(p => p.FechaInicio)
                .Take(top)
                .ToListAsync();

            return pagos;
        }

        public async Task<bool> ValidarCuentaYCategoria(int cuentaId, int categoriaId, int usuarioId)
        {
            var cuentaValida = await _db.Cuentas
                .AnyAsync(c => c.CuentaId == cuentaId && c.UsuarioId == usuarioId);

            var categoriaValida = await _db.Categorias
                .AnyAsync(c => c.CategoriaId == categoriaId && c.UsuarioId == usuarioId);

            return cuentaValida && categoriaValida;
        }

        public async Task<bool> ActualizarProximaEjecucion(int pagoHabitualId, DateTime proximaEjecucion)
        {
            var pago = await _db.PagosHabituales.FindAsync(pagoHabitualId);
            if (pago == null) return false;

            // Aquí podrías agregar lógica para calcular la próxima ejecución
            // basada en la frecuencia y unidad de frecuencia
            pago.FechaInicio = proximaEjecucion;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<PagosHabituales?> ObtenerPorIdConRelaciones(int id)
        {
            return await _db.PagosHabituales
                .Include(p => p.Cuenta)
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.PagoHabitualId == id);
        }
    }
}