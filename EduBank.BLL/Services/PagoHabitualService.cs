// EduBank.BLL/Services/PagoHabitualService.cs
using EduBank.DAL.Repository;
using EduBank.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduBank.BLL.Services
{
    public class PagoHabitualService : IPagoHabitualService
    {
        private readonly IPagoHabitualRepository _pagoRepo;
        private readonly IGenericRepository<PagosHabituales> _genericRepo;
        private readonly IMovimientoService _movimientoService;
        private readonly ILogger<PagoHabitualService> _logger;

        public PagoHabitualService(
            IPagoHabitualRepository pagoRepo,
            IGenericRepository<PagosHabituales> genericRepo,
            IMovimientoService movimientoService,
            ILogger<PagoHabitualService> logger)
        {
            _pagoRepo = pagoRepo;
            _genericRepo = genericRepo;
            _movimientoService = movimientoService;
            _logger = logger;
        }

        public async Task<bool> Insertar(PagosHabituales modelo)
        {
            try
            {
                // Validar que la cuenta y categoría pertenezcan al usuario
                var pertenece = await _pagoRepo.ValidarCuentaYCategoria(modelo.CuentaId, modelo.CategoriaId, modelo.UsuarioId);
                if (!pertenece)
                    throw new Exception("La cuenta o categoría no pertenece al usuario");

                // Validar fecha de fin
                if (modelo.FechaFin.HasValue && modelo.FechaFin <= modelo.FechaInicio)
                    throw new Exception("La fecha de fin debe ser posterior a la fecha de inicio");

                return await _genericRepo.Insertar(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar pago habitual");
                throw;
            }
        }

        // EduBank.BLL/Services/PagoHabitualService.cs
        public async Task<bool> Actualizar(PagosHabituales modelo)
        {
            try
            {
                // Obtener el pago existente
                var pagoExistente = await _genericRepo.Obtener(modelo.PagoHabitualId);
                if (pagoExistente == null)
                    throw new Exception("Pago habitual no encontrado");

                if (pagoExistente.UsuarioId != modelo.UsuarioId)
                    throw new Exception("No tiene permisos para editar este pago");

                // Validar cuenta y categoría
                var pertenece = await _pagoRepo.ValidarCuentaYCategoria(modelo.CuentaId, modelo.CategoriaId, modelo.UsuarioId);
                if (!pertenece)
                    throw new Exception("La cuenta o categoría no pertenece al usuario");

                if (modelo.FechaFin.HasValue && modelo.FechaFin <= modelo.FechaInicio)
                    throw new Exception("La fecha de fin debe ser posterior a la fecha de inicio");

                // ✅ CORRECCIÓN: Actualizar propiedades del pago existente
                pagoExistente.Nombre = modelo.Nombre;
                pagoExistente.Frecuencia = modelo.Frecuencia;
                pagoExistente.UnidadFrecuencia = modelo.UnidadFrecuencia;
                pagoExistente.FechaInicio = modelo.FechaInicio;
                pagoExistente.Hora = modelo.Hora;
                pagoExistente.FechaFin = modelo.FechaFin;
                pagoExistente.CuentaId = modelo.CuentaId;
                pagoExistente.CategoriaId = modelo.CategoriaId;
                pagoExistente.Monto = modelo.Monto;
                pagoExistente.Comentario = modelo.Comentario;
                pagoExistente.EsActivo = modelo.EsActivo;
                pagoExistente.AgregarAutomaticamente = modelo.AgregarAutomaticamente;

                // ✅ Actualizar la entidad existente
                return await _genericRepo.Actualizar(pagoExistente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar pago habitual ID: {PagoId}", modelo.PagoHabitualId);
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                return await _genericRepo.Eliminar(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar pago habitual ID: {PagoId}", id);
                throw;
            }
        }

        public async Task<PagosHabituales?> Obtener(int id)
        {
            // En lugar de usar _genericRepo, usa el repositorio específico
            var pagos = await _pagoRepo.ObtenerPorUsuario(0); // Obtener todos temporalmente
            return pagos.FirstOrDefault(p => p.PagoHabitualId == id);
        }

        public async Task<IEnumerable<PagosHabituales>> ObtenerPorUsuario(int usuarioId)
        {
            return await _pagoRepo.ObtenerPorUsuario(usuarioId);
        }

        public async Task<IEnumerable<PagosHabituales>> ObtenerActivosPorUsuario(int usuarioId)
        {
            var pagos = await _pagoRepo.ObtenerPorUsuario(usuarioId);
            return pagos.Where(p => p.EsActivo &&
                (!p.FechaFin.HasValue || p.FechaFin.Value >= DateTime.Today));
        }

        public async Task<bool> CambiarEstado(int id, bool activo)
        {
            try
            {
                var pago = await _genericRepo.Obtener(id);
                if (pago == null) return false;

                pago.EsActivo = activo;
                return await _genericRepo.Actualizar(pago);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del pago habitual ID: {PagoId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<PagosHabituales>> ObtenerPagosParaEjecutar(DateTime fechaReferencia)
        {
            return await _pagoRepo.ObtenerPagosParaEjecutar(fechaReferencia);
        }

        public async Task<bool> EjecutarPagoAutomatico(int pagoHabitualId)
        {
            try
            {
                var pago = await _genericRepo.Obtener(pagoHabitualId);
                if (pago == null || !pago.AgregarAutomaticamente || !pago.EsActivo)
                    return false;

                // Crear movimiento automático
                var movimiento = new Movimiento
                {
                    CuentaId = pago.CuentaId,
                    CategoriaId = pago.CategoriaId,
                    Tipo = "G", // Gastos
                    FechaOperacion = DateTime.Now,
                    Monto = pago.Monto,
                    Comentario = $"Pago automático: {pago.Nombre}",
                    EsAutomatico = true,
                    CreadoEn = DateTime.Now,
                    ActualizadoEn = DateTime.Now
                };

                var resultado = await _movimientoService.Insertar(movimiento);

                if (resultado)
                {
                    _logger.LogInformation("Pago automático ejecutado: {PagoNombre} - {Monto}", pago.Nombre, pago.Monto);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar pago automático ID: {PagoId}", pagoHabitualId);
                return false;
            }
        }

        public async Task<IEnumerable<PagosHabituales>> ObtenerProximosPagos(int usuarioId, int top = 5)
        {
            var pagos = await _pagoRepo.ObtenerProximosPagos(usuarioId, top);
            return pagos.Where(p => p.EsActivo);
        }

        public async Task<decimal> ObtenerTotalPagosProximos(int usuarioId)
        {
            var pagos = await ObtenerProximosPagos(usuarioId);
            return pagos.Sum(p => p.Monto);
        }

        public async Task<bool> PerteneceAUsuario(int pagoHabitualId, int usuarioId)
        {
            var pago = await _genericRepo.Obtener(pagoHabitualId);
            return pago?.UsuarioId == usuarioId;
        }


    }
}