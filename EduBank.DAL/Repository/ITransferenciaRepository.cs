using EduBank.DAL.DataContext;
using EduBank.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduBank.DAL.Repository
{
    public interface ITransferenciaRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorUsuario(int usuarioId);
        Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorCuenta(int cuentaId);
        Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorPeriodo(int usuarioId, DateTime desde, DateTime hasta);
    }

    public class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly EdubanckssrContext _db;

        public TransferenciaRepository(EdubanckssrContext db)
        {
            _db = db;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }

        public async Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorUsuario(int usuarioId)
        {
            return await _db.Transferencias
                .Include(t => t.CuentaOrigen)
                .Include(t => t.CuentaDestino)
                .Where(t => t.CuentaOrigen.UsuarioId == usuarioId || t.CuentaDestino.UsuarioId == usuarioId)
                .OrderByDescending(t => t.FechaTransferencia)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorCuenta(int cuentaId)
        {
            return await _db.Transferencias
                .Include(t => t.CuentaOrigen)
                .Include(t => t.CuentaDestino)
                .Where(t => t.CuentaOrigenId == cuentaId || t.CuentaDestinoId == cuentaId)
                .OrderByDescending(t => t.FechaTransferencia)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorPeriodo(int usuarioId, DateTime desde, DateTime hasta)
        {
            return await _db.Transferencias
                .Include(t => t.CuentaOrigen)
                .Include(t => t.CuentaDestino)
                .Where(t => (t.CuentaOrigen.UsuarioId == usuarioId || t.CuentaDestino.UsuarioId == usuarioId) &&
                           t.FechaTransferencia >= desde && t.FechaTransferencia <= hasta)
                .OrderByDescending(t => t.FechaTransferencia)
                .ToListAsync();
        }
    }
}