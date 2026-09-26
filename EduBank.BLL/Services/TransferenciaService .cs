using EduBank.DAL.Repository;
using EduBank.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduBank.BLL.Services
{
    public class TransferenciaService : ITransferenciaService
    {
        private readonly ITransferenciaRepository _transferenciaRepo;
        private readonly ICuentaRepository _cuentaRepo;
        private readonly IGenericRepository<Transferencia> _genericRepo;

        public TransferenciaService(
            ITransferenciaRepository transferenciaRepo,
            ICuentaRepository cuentaRepo,
            IGenericRepository<Transferencia> genericRepo)
        {
            _transferenciaRepo = transferenciaRepo;
            _cuentaRepo = cuentaRepo;
            _genericRepo = genericRepo;
        }

        public async Task<bool> RealizarTransferencia(Transferencia transferencia)
        {
            // Usar using para asegurar disposición automática
            using var transaction = await _transferenciaRepo.BeginTransactionAsync();

            try
            {
                // 1. Validar que las cuentas existen
                var cuentaOrigen = await _cuentaRepo.Obtener(transferencia.CuentaOrigenId);
                var cuentaDestino = await _cuentaRepo.Obtener(transferencia.CuentaDestinoId);

                if (cuentaOrigen == null || cuentaDestino == null)
                    throw new Exception("Una de las cuentas no existe");

                // 2. Validar que pertenecen al mismo usuario
                if (cuentaOrigen.UsuarioId != cuentaDestino.UsuarioId)
                    throw new Exception("Las transferencias solo pueden realizarse entre cuentas del mismo usuario");

                // 3. Validar que las cuentas estén activas
                if (!cuentaOrigen.Activo || !cuentaDestino.Activo)
                    throw new Exception("Ambas cuentas deben estar activas");

                // 4. Validar que no sea la misma cuenta
                if (transferencia.CuentaOrigenId == transferencia.CuentaDestinoId)
                    throw new Exception("No se puede transferir a la misma cuenta");

                // 5. Validar saldo suficiente
                if (cuentaOrigen.Saldo < transferencia.Monto)
                    throw new Exception($"Saldo insuficiente. Saldo disponible: {cuentaOrigen.Saldo:C}");

                // 6. Validar monto positivo
                if (transferencia.Monto <= 0)
                    throw new Exception("El monto debe ser mayor a cero");

                // 7. Actualizar saldos
                cuentaOrigen.Saldo -= transferencia.Monto;
                cuentaDestino.Saldo += transferencia.Monto;

                await _cuentaRepo.ActualizarSaldo(cuentaOrigen.CuentaId, cuentaOrigen.Saldo);
                await _cuentaRepo.ActualizarSaldo(cuentaDestino.CuentaId, cuentaDestino.Saldo);

                // 8. Registrar la transferencia
                transferencia.FechaTransferencia = DateTime.Now;
                var resultado = await _genericRepo.Insertar(transferencia);

                await transaction.CommitAsync();
                return resultado;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> RevertirTransferencia(int transferenciaId)
        {
            using var transaction = await _transferenciaRepo.BeginTransactionAsync();

            try
            {
                var transferencia = await _genericRepo.Obtener(transferenciaId);
                if (transferencia == null)
                    throw new Exception("Transferencia no encontrada");

                var cuentaOrigen = await _cuentaRepo.Obtener(transferencia.CuentaOrigenId);
                var cuentaDestino = await _cuentaRepo.Obtener(transferencia.CuentaDestinoId);

                if (cuentaDestino.Saldo < transferencia.Monto)
                    throw new Exception("Saldo insuficiente para revertir la transferencia");

                // Revertir saldos
                cuentaOrigen.Saldo += transferencia.Monto;
                cuentaDestino.Saldo -= transferencia.Monto;

                await _cuentaRepo.ActualizarSaldo(cuentaOrigen.CuentaId, cuentaOrigen.Saldo);
                await _cuentaRepo.ActualizarSaldo(cuentaDestino.CuentaId, cuentaDestino.Saldo);

                // Eliminar la transferencia (o marcar como revertida)
                var resultado = await _genericRepo.Eliminar(transferenciaId);

                await transaction.CommitAsync();
                return resultado;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Transferencia?> ObtenerTransferencia(int id)
        {
            return await _genericRepo.Obtener(id);
        }

        public async Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorUsuario(int usuarioId)
        {
            return await _transferenciaRepo.ObtenerTransferenciasPorUsuario(usuarioId);
        }

        public async Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorCuenta(int cuentaId)
        {
            return await _transferenciaRepo.ObtenerTransferenciasPorCuenta(cuentaId);
        }

        public async Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorPeriodo(int usuarioId, DateTime desde, DateTime hasta)
        {
            return await _transferenciaRepo.ObtenerTransferenciasPorPeriodo(usuarioId, desde, hasta);
        }

        public async Task<decimal> ObtenerTotalTransferenciasPorPeriodo(int usuarioId, DateTime desde, DateTime hasta)
        {
            var transferencias = await _transferenciaRepo.ObtenerTransferenciasPorPeriodo(usuarioId, desde, hasta);
            return transferencias.Sum(t => t.Monto);
        }
    }
}