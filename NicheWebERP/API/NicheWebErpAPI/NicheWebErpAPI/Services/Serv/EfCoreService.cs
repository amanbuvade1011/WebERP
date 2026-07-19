using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace NicheWebErpAPI
{ 
    public class EfCoreService<T> : IEfCoreService<T> where T : class
    {
        private readonly ERPDbContext _context;
        private readonly DbSet<T> _dbSet;

        public EfCoreService(ERPDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Query() => _dbSet.AsQueryable();

        public async Task<T?> GetByIdAsync(params object[] keyValues) => await _dbSet.FindAsync(keyValues);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
