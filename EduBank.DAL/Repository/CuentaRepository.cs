using EduBank.DAL.DataContext;
using EduBank.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduBank.DAL.Repository
{
    public class CuentaRepository : GenericRepository<Cuenta>, ICuentaRepository
    {
        private readonly EdubanckssrContext _dbContext;

        public CuentaRepository(EdubanckssrContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Cuenta>> ObtenerPorUsuario(int usuarioId)
        {
            return await _dbContext.Cuentas
                .Where(c => c.UsuarioId == usuarioId && c.Activo)
                .Include(c => c.Usuario)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<decimal> ObtenerSaldoTotal(int usuarioId)
        {
            return await _dbContext.Cuentas
                .Where(c => c.UsuarioId == usuarioId && c.Activo)
                .SumAsync(c => c.Saldo);
        }

        public async Task<bool> ActualizarSaldo(int cuentaId, decimal nuevoSaldo)
        {
            var cuenta = await _dbContext.Cuentas.FindAsync(cuentaId);
            if (cuenta != null)
            {
                cuenta.Saldo = nuevoSaldo;
                _dbContext.Cuentas.Update(cuenta);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> PerteneceAUsuario(int cuentaId, int usuarioId)
        {
            return await _dbContext.Cuentas
                .AnyAsync(c => c.CuentaId == cuentaId && c.UsuarioId == usuarioId);
        }

        // NUEVA IMPLEMENTACIÓN
        public async Task<Cuenta?> ObtenerPorIdYUsuario(int id, int usuarioId)
        {
            return await _dbContext.Cuentas
                .Where(c => c.CuentaId == id && c.UsuarioId == usuarioId)
                .FirstOrDefaultAsync();
        }
    }
}