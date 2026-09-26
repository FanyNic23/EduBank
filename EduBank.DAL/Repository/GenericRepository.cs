using EduBank.DAL.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduBank.DAL.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly EdubanckssrContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ILogger<GenericRepository<TEntity>>? _logger; // <- opcional

        // El logger es opcional
        public GenericRepository(EdubanckssrContext dbContext, ILogger<GenericRepository<TEntity>>? logger = null)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
            _logger = logger;
        }

        public async Task<bool> Insertar(TEntity modelo)
        {
            try
            {
                await _dbSet.AddAsync(modelo);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al insertar entidad {Entity}", typeof(TEntity).Name);
                throw;
            }
        }

        public async Task<bool> Actualizar(TEntity modelo)
        {
            try
            {
                _dbSet.Update(modelo);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al actualizar entidad {Entity}", typeof(TEntity).Name);
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);
                if (entidad == null) return false;

                _dbSet.Remove(entidad);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al eliminar entidad {Entity}", typeof(TEntity).Name);
                throw;
            }
        }

        public async Task<TEntity?> Obtener(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<TEntity>> ObtenerTodos()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
