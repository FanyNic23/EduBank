// EduBank/DAL/Repository/ICuentaRepository.cs
using EduBank.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduBank.DAL.Repository
{
    public interface ICuentaRepository : IGenericRepository<Cuenta>
    {
        Task<IEnumerable<Cuenta>> ObtenerPorUsuario(int usuarioId);
        Task<decimal> ObtenerSaldoTotal(int usuarioId);
        Task<bool> ActualizarSaldo(int cuentaId, decimal nuevoSaldo);
        Task<bool> PerteneceAUsuario(int cuentaId, int usuarioId);
        // NUEVO MÉTODO
        Task<Cuenta?> ObtenerPorIdYUsuario(int cuentaId, int usuarioId);
    }
}