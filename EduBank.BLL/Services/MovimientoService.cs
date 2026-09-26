using EduBank.DAL.Repository;
using EduBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.BLL.Services
{
    public interface IMovimientoService
    {
        Task<bool> Insertar(Movimiento modelo);
        Task<bool> Actualizar(Movimiento modelo);

        // MÉTODOS SEGUROS
        Task<bool> Eliminar(long id, int usuarioId);
        Task<Movimiento?> Obtener(long id, int usuarioId);
        Task<List<Movimiento>> ObtenerPorUsuario(int usuarioId);

        // ELIMINAR métodos inseguros
        // Task<bool> Eliminar(long id);
        // Task<Movimiento?> Obtener(long id);
        // Task<List<Movimiento>> ObtenerTodos();

        Task<decimal> ObtenerTotalPorTipo(char tipo, int usuarioId);
        Task<IEnumerable<(int CategoriaId, string CategoriaNombre, decimal Total)>> ObtenerTotalesPorCategoria(int usuarioId, char? tipo = null);
        Task<IEnumerable<Movimiento>> ObtenerRecientes(int usuarioId, int top = 10);
        Task<IEnumerable<Movimiento>> ObtenerPorRango(int usuarioId, DateTime desde, DateTime hasta);
        Task<IEnumerable<Movimiento>> ObtenerPorPeriodo(int usuarioId, string periodo, DateTime fechaReferencia);
        Task<(DateTime Inicio, DateTime Fin)> ObtenerRangoPeriodo(string periodo, DateTime fechaReferencia);
    }
}
