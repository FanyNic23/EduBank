// EduBank/BLL/Services/ICuentaService.cs
using EduBank.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduBank.BLL.Services
{
    public interface ICuentaService
    {
        Task<bool> Insertar(Cuenta modelo);
        Task<bool> Actualizar(Cuenta modelo);
        Task<bool> Eliminar(int id);
        Task<Cuenta?> Obtener(int id);
        Task<List<Cuenta>> ObtenerTodos();
        Task<IEnumerable<Cuenta>> ObtenerPorUsuario(int usuarioId);
        Task<decimal> ObtenerSaldoTotal(int usuarioId);
        Task<bool> ActualizarSaldo(int cuentaId, decimal nuevoSaldo);

        // NUEVO MÉTODO - Para validar que la cuenta pertenece al usuario
        Task<Cuenta?> ObtenerPorIdYUsuario(int cuentaId, int usuarioId);
    }
}