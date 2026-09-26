// EduBank/BLL/Services/CuentaService.cs
using EduBank.DAL.Repository;
using EduBank.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduBank.BLL.Services
{
    public class CuentaService : ICuentaService
    {
        private readonly ICuentaRepository _repo;

        public CuentaService(ICuentaRepository repo)
        {
            _repo = repo;
        }

        public Task<bool> Insertar(Cuenta modelo) => _repo.Insertar(modelo);
        public Task<bool> Actualizar(Cuenta modelo) => _repo.Actualizar(modelo);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
        public Task<Cuenta?> Obtener(int id) => _repo.Obtener(id);
        public Task<List<Cuenta>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<IEnumerable<Cuenta>> ObtenerPorUsuario(int usuarioId) => _repo.ObtenerPorUsuario(usuarioId);
        public Task<decimal> ObtenerSaldoTotal(int usuarioId) => _repo.ObtenerSaldoTotal(usuarioId);
        public Task<bool> ActualizarSaldo(int cuentaId, decimal nuevoSaldo) => _repo.ActualizarSaldo(cuentaId, nuevoSaldo);

        // NUEVO MÉTODO
        public Task<Cuenta?> ObtenerPorIdYUsuario(int cuentaId, int usuarioId) => _repo.ObtenerPorIdYUsuario(cuentaId, usuarioId);
    }
}
