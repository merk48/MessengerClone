using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.IRepository;
using MessengerClone.Repository.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;


namespace MessengerClone.Repository.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        #region Get
        public TEntity? Get(Expression<Func<TEntity, bool>> filter)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(filter);
        }
       
        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(filter);
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter,Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (include != null)
                query = include(query);

            return await query.AsNoTracking().FirstOrDefaultAsync(filter);
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null)
        {
            return filter == null
                ? _dbSet.AsNoTracking()
                : _dbSet.AsNoTracking().Where(filter);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            return filter == null
            ? await _dbSet.AsNoTracking().ToListAsync()
            : await _dbSet.AsNoTracking().Where(filter).ToListAsync();
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }


        public IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            // Validate input
            if (includeProperties == null || includeProperties.Length == 0)
            {
                throw new ArgumentException("Include properties cannot be null or empty.", nameof(includeProperties));
            }

            IQueryable<TEntity> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                // Validate each includeProperty
                if (includeProperty == null)
                {
                    throw new ArgumentException("One of the include properties is null.", nameof(includeProperties));
                }

                try
                {
                    query = query.Include(includeProperty);
                }
                catch (Exception ex)
                {
                    // Re-throw the exception or handle it as needed
                    throw new InvalidOperationException($"Failed to include property {includeProperty}.", ex);
                }
            }

            return query;
        }

        public IQueryable<TEntity> Table => _dbSet.AsQueryable();

        #endregion

        #region Add
        public void Add(TEntity entity)
        {
            _OnAdd(entity);
            _dbSet.Add(entity);
        }

        public async Task AddAsync(TEntity entity)
        {
            _OnAdd(entity);
            await _dbSet.AddAsync(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _OnAdd(entity);
            }
            _dbSet.AddRange(entities);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _OnAdd(entity);
            }
            await _dbSet.AddRangeAsync(entities);
        }

        private void _OnAdd(TEntity entity)
        {
            if (entity is ICreateAt createAt)
            {
                createAt.CreatedAt = DateTime.UtcNow;
            }
        }

        #endregion

        #region Update
        public void Update(TEntity entity)
        {
            _OnUpdate(entity);
            _dbSet.Update(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _OnUpdate(entity);
            await Task.Run(() => _dbSet.Update(entity));
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _OnUpdate(entity);
            }
            _dbSet.UpdateRange(entities);
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _OnUpdate(entity);
            }
            await Task.Run(() => _dbSet.UpdateRange(entities));
        }
       
        private void _OnUpdate(TEntity entity)
        {
            if (entity is IUpdateAt updateAt)
            {
                updateAt.UpdatedAt = DateTime.UtcNow;
            }
        }

        #endregion

        #region Delete
        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(() => _dbSet.Remove(entity));
        }

        public void Delete(Expression<Func<TEntity, bool>> filter)
        {
            _dbSet.RemoveRange(_dbSet.Where(filter));
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            await Task.Run(() =>
            {
                var entities = _dbSet.Where(filter);
                _dbSet.RemoveRange(entities);
            });
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() => _dbSet.RemoveRange(entities));
        }

        public void SoftDelete(TEntity entity)
        {
            if(entity is ISoftDeletable softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = true;
                softDeletableEntity.DateDeleted = DateTime.Now;
                Update(entity);
            }
            else
            {
                throw new InvalidOperationException("Entity does not support soft delete.");
            }
        }

        public void SoftDelete(Expression<Func<TEntity, bool>> filter)
        {
            foreach (var entity in _dbSet.Where(filter).ToList())
            {
                SoftDelete(entity);
            }
        }

        public async Task SoftDeleteAsync(TEntity entity)
        {
            if (entity is ISoftDeletable softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = true;
                softDeletableEntity.DateDeleted = DateTime.Now;
                await UpdateAsync(entity);
            }
            else
            {
                throw new InvalidOperationException("Entity does not support soft delete.");
            }
        }

        public async Task SoftDeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            var entities = await _dbSet.Where(filter).ToListAsync();
            foreach (var entity in entities)
            {
                await SoftDeleteAsync(entity);
            }
        }

        #endregion


    }
}
