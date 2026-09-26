using EduBank.DAL.DataContext;
using EduBank.DAL.Repository;
using EduBank.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduBank.BLL.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IMovimientoRepository _movimientoRepo;
        private readonly ICuentaRepository _cuentaRepo;
        private readonly EdubanckssrContext _dbContext;

        public MovimientoService(IMovimientoRepository movimientoRepo,
                               ICuentaRepository cuentaRepo,
                               EdubanckssrContext dbContext)
        {
            _movimientoRepo = movimientoRepo;
            _cuentaRepo = cuentaRepo;
            _dbContext = dbContext;
        }

        public async Task<bool> Insertar(Movimiento modelo)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1. Verificar que la cuenta existe y pertenece al usuario
                var cuenta = await _cuentaRepo.ObtenerPorIdYUsuario(modelo.CuentaId, modelo.Cuenta.UsuarioId);
                if (cuenta == null) return false;

                // 2. Insertar el movimiento
                var resultado = await _movimientoRepo.Insertar(modelo);
                if (!resultado) return false;

                // 3. Actualizar saldo de la cuenta
                decimal nuevoSaldo = modelo.Tipo == "I"
                    ? cuenta.Saldo + modelo.Monto  // Ingreso: sumar
                    : cuenta.Saldo - modelo.Monto; // Gasto: restar

                await _cuentaRepo.ActualizarSaldo(modelo.CuentaId, nuevoSaldo);

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> Actualizar(Movimiento modelo)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1. Obtener movimiento original
                var movimientoOriginal = await _movimientoRepo.ObtenerPorUsuario(
                    modelo.MovimientoId, modelo.Cuenta.UsuarioId);
                if (movimientoOriginal == null) return false;

                // 2. Revertir saldo del movimiento original
                var cuenta = await _cuentaRepo.ObtenerPorIdYUsuario(
                    movimientoOriginal.CuentaId, movimientoOriginal.Cuenta.UsuarioId);
                if (cuenta == null) return false;

                decimal saldoRevertido = movimientoOriginal.Tipo == "I"
                    ? cuenta.Saldo - movimientoOriginal.Monto  // Revertir ingreso
                    : cuenta.Saldo + movimientoOriginal.Monto; // Revertir gasto

                // 3. Aplicar nuevo movimiento
                decimal nuevoSaldo = modelo.Tipo == "I"
                    ? saldoRevertido + modelo.Monto  // Nuevo ingreso
                    : saldoRevertido - modelo.Monto; // Nuevo gasto

                // 4. Actualizar movimiento
                var resultado = await _movimientoRepo.Actualizar(modelo);
                if (!resultado) return false;

                // 5. Actualizar saldo
                await _cuentaRepo.ActualizarSaldo(modelo.CuentaId, nuevoSaldo);

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> Eliminar(long id, int usuarioId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1. Obtener movimiento
                var movimiento = await _movimientoRepo.ObtenerPorUsuario(id, usuarioId);
                if (movimiento == null) return false;

                // 2. Obtener cuenta
                var cuenta = await _cuentaRepo.ObtenerPorIdYUsuario(
                    movimiento.CuentaId, usuarioId);
                if (cuenta == null) return false;

                // 3. Revertir saldo
                decimal nuevoSaldo = movimiento.Tipo == "I"
                    ? cuenta.Saldo - movimiento.Monto  // Revertir ingreso
                    : cuenta.Saldo + movimiento.Monto; // Revertir gasto

                // 4. Eliminar movimiento
                var resultado = await _movimientoRepo.EliminarPorUsuario(id, usuarioId);
                if (!resultado) return false;

                // 5. Actualizar saldo
                await _cuentaRepo.ActualizarSaldo(movimiento.CuentaId, nuevoSaldo);

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // --------------------
        public Task<Movimiento?> Obtener(long id, int usuarioId) => _movimientoRepo.ObtenerPorUsuario(id, usuarioId);
        public Task<List<Movimiento>> ObtenerPorUsuario(int usuarioId) => _movimientoRepo.ObtenerPorUsuario(usuarioId);
        public Task<decimal> ObtenerTotalPorTipo(char tipo, int usuarioId) => _movimientoRepo.ObtenerTotalPorTipo(tipo, usuarioId);
        public Task<IEnumerable<(int CategoriaId, string CategoriaNombre, decimal Total)>> ObtenerTotalesPorCategoria(int usuarioId, char? tipo = null) => _movimientoRepo.ObtenerTotalesPorCategoria(usuarioId, tipo);
        public Task<IEnumerable<Movimiento>> ObtenerRecientes(int usuarioId, int top = 10) => _movimientoRepo.ObtenerRecientes(usuarioId, top);
        public Task<IEnumerable<Movimiento>> ObtenerPorRango(int usuarioId, DateTime desde, DateTime hasta) => _movimientoRepo.ObtenerPorRango(usuarioId, desde, hasta);
        public Task<IEnumerable<Movimiento>> ObtenerPorPeriodo(int usuarioId, string periodo, DateTime fechaReferencia) => _movimientoRepo.ObtenerPorPeriodo(usuarioId, periodo, fechaReferencia);
        public Task<(DateTime Inicio, DateTime Fin)> ObtenerRangoPeriodo(string periodo, DateTime fechaReferencia) => _movimientoRepo.ObtenerRangoPeriodo(periodo, fechaReferencia);
    }
}