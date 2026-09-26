using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.DAL.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<bool> Insertar(TEntity modelo);
        Task<bool> Actualizar(TEntity modelo);
        Task<bool> Eliminar(int id);
        Task<TEntity?> Obtener(int id);
        Task<List<TEntity>> ObtenerTodos();
    }
}
