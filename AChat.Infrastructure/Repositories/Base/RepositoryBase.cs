using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using AChat.Domain.Entities.Base;
using AChat.Domain.Repositories.Base;
using AChat.Infrastructure.Data;

namespace AChat.Infrastructure.Repositories.Base;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{
    private DbSet<TEntity>? _dbSet;
    protected DbSet<TEntity> DbSet => _dbSet ??= _dbContext.Set<TEntity>();
    
    private readonly AppDbContext _dbContext;
    
    public RepositoryBase(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return DbSet.AsQueryable().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public IQueryable<TEntity> GetById(int id) => DbSet.Where(e => e.Id == id);

    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return DbSet.Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
    {
        return DbSet.Where(predicate).ToListAsync(ct);
    }

    public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>>? predicate = null)
        => predicate is null ? DbSet.AsQueryable() : DbSet.Where(predicate);
        

    public void Add(TEntity entity) => DbSet.Add(entity);
    
    public void Update(TEntity entity) => DbSet.Update(entity);
    
    public void Delete(TEntity entity) => DbSet.Remove(entity);
    
    public void DeleteRange(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);
    
    public Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(e => e.Id == id, cancellationToken);

    public Task<bool> IsAllExistAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        => DbSet.AllAsync(e => ids.Contains(e.Id), cancellationToken);
}
