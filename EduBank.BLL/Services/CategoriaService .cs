using EduBank.DAL.Repository;
using EduBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.BLL.Services
{

    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Categoria> _repo;
        private readonly CategoriaRepository _categoriaRepo;

        public CategoriaService(IGenericRepository<Categoria> repo, CategoriaRepository categoriaRepo)
        {
            _repo = repo;
            _categoriaRepo = categoriaRepo;
        }

        public Task<bool> Insertar(Categoria modelo) => _repo.Insertar(modelo);
        public Task<bool> Actualizar(Categoria modelo) => _repo.Actualizar(modelo);

        // Hard Delete (eliminación física)
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);

        // Nuevo: Cambiar estado (activar/desactivar)
        public Task<bool> CambiarEstado(int id, bool activo) => _categoriaRepo.CambiarEstado(id, activo);

        public Task<Categoria?> Obtener(int id) => _repo.Obtener(id);
        public Task<List<Categoria>> ObtenerTodos() => _repo.ObtenerTodos();

        public Task<Categoria?> ObtenerPorIdYUsuario(int id, int usuarioId)
            => _categoriaRepo.ObtenerPorIdYUsuario(id, usuarioId);

        public Task<List<Categoria>> ObtenerPorUsuario(int usuarioId)
            => _categoriaRepo.ObtenerPorUsuario(usuarioId);
    }
}
