using System.Linq.Expressions;
using Infrastructure.Data.DbContext;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
	public class Repository<TEntity>: IRepository<TEntity> where TEntity: class
	{
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public Task<TEntity> AddAndReturnValue(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await _context.Set<TEntity>().AddAsync(entity);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Any(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate);
        }

        public Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public Task<TEntity> FirstOrDefaultNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public Task<TEntity> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression)
        {
            return _context.Set<TEntity>().AsQueryable().Where(expression);
        }

        public void Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> SingleOrDefaultNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<TEntity> entity)
        {
            throw new NotImplementedException();
        }
    }
}

