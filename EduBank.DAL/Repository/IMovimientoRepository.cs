using EduBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.DAL.Repository
{
    public interface IMovimientoRepository : IGenericRepository<Movimiento>
    {
        // AGREGAR usuarioId a todos los métodos
        Task<decimal> ObtenerTotalPorTipo(char tipo, int usuarioId);
        Task<IEnumerable<(int CategoriaId, string CategoriaNombre, decimal Total)>> ObtenerTotalesPorCategoria(int usuarioId, char? tipo = null);
        Task<IEnumerable<Movimiento>> ObtenerRecientes(int usuarioId, int top = 10);
        Task<IEnumerable<Movimiento>> ObtenerPorRango(int usuarioId, DateTime desde, DateTime hasta);
        Task<IEnumerable<Movimiento>> ObtenerPorPeriodo(int usuarioId, string periodo, DateTime fechaReferencia);

        // MÉTODOS NUEVOS para operaciones seguras
        Task<Movimiento?> ObtenerPorUsuario(long id, int usuarioId);
        Task<List<Movimiento>> ObtenerPorUsuario(int usuarioId);
        Task<bool> EliminarPorUsuario(long id, int usuarioId);

        // Mantener sin cambios
        Task<(DateTime Inicio, DateTime Fin)> ObtenerRangoPeriodo(string periodo, DateTime fechaReferencia);
    }
}
