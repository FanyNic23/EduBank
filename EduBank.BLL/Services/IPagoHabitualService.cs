// EduBank.BLL/Services/IPagoHabitualService.cs
using EduBank.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduBank.BLL.Services
{
    public interface IPagoHabitualService
    {
        // CRUD Básico
        Task<bool> Insertar(PagosHabituales modelo);
        Task<bool> Actualizar(PagosHabituales modelo);
        Task<bool> Eliminar(int id);
        Task<PagosHabituales?> Obtener(int id);
        Task<IEnumerable<PagosHabituales>> ObtenerPorUsuario(int usuarioId);
        Task<IEnumerable<PagosHabituales>> ObtenerActivosPorUsuario(int usuarioId);

        // Funcionalidades Específicas
        Task<bool> CambiarEstado(int id, bool activo);
        Task<IEnumerable<PagosHabituales>> ObtenerPagosParaEjecutar(DateTime fechaReferencia);
        Task<bool> EjecutarPagoAutomatico(int pagoHabitualId);
        Task<IEnumerable<PagosHabituales>> ObtenerProximosPagos(int usuarioId, int top = 5);
        Task<decimal> ObtenerTotalPagosProximos(int usuarioId);

        // Validaciones
        Task<bool> PerteneceAUsuario(int pagoHabitualId, int usuarioId);
    }
}