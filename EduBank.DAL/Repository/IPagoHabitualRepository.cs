// EduBank.DAL/Repository/IPagoHabitualRepository.cs
using EduBank.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduBank.DAL.Repository
{
    public interface IPagoHabitualRepository
    {
        Task<IEnumerable<PagosHabituales>> ObtenerPorUsuario(int usuarioId);
        Task<IEnumerable<PagosHabituales>> ObtenerPagosParaEjecutar(DateTime fechaReferencia);
        Task<IEnumerable<PagosHabituales>> ObtenerProximosPagos(int usuarioId, int top = 5);
        Task<bool> ValidarCuentaYCategoria(int cuentaId, int categoriaId, int usuarioId);
        Task<bool> ActualizarProximaEjecucion(int pagoHabitualId, DateTime proximaEjecucion);

        Task<PagosHabituales?> ObtenerPorIdConRelaciones(int id);
    }
}