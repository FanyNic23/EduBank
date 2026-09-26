    using EduBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.BLL.Services
{
    public interface ICategoriaService
    {
        Task<bool> Insertar(Categoria modelo);
        Task<bool> Actualizar(Categoria modelo);
        Task<bool> Eliminar(int id); // Ahora es hard delete
        Task<bool> CambiarEstado(int id, bool activo); // Nuevo método para soft delete
        Task<Categoria?> Obtener(int id);
        Task<List<Categoria>> ObtenerTodos();

        Task<Categoria?> ObtenerPorIdYUsuario(int id, int usuarioId);
        Task<List<Categoria>> ObtenerPorUsuario(int usuarioId);
    }
}
