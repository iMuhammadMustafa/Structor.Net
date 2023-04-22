using System.Linq.Expressions;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class Repository<TEntity, TContext> : IRepository<TEntity>
    where TEntity : IEntity
    where TContext : DbContext
{
    private readonly TContext _context;

    public Repository(TContext context)
    {
        _context = context;
    }

    private DbSet<TEntity>? _dbSet;

    protected DbSet<TEntity> DbSet
    {
        get
        {
            if (_dbSet == null)
                _dbSet = _context.Set<TEntity>();
            return _dbSet;
        }
    }


    /* */
    public virtual IQueryable<TEntity> GetAll() => DbSet.AsQueryable();

    public virtual IQueryable<TEntity> GetIncluding(params Expression<Func<TEntity, object>>[] included)
    {
        var queryable = DbSet.AsQueryable();

        foreach (var include in included)
        {
            queryable = queryable.Include(include);
        }
        return queryable;
    }

    public virtual async Task<TEntity> GetById(int id) => await DbSet.FirstOrDefaultAsync(Entity => Entity.Id == id) ?? null!;

    public virtual async Task<TEntity> GetByGuid(Guid guid) => await DbSet.FirstOrDefaultAsync(Entity => Entity.Guid == guid) ?? null!;

    public virtual async Task<int> GetCount() => await DbSet.CountAsync();

    public virtual async Task<int> GetCountWhere(Expression<Func<TEntity, bool>> expression) => await DbSet.Where(expression).CountAsync();


    public virtual async Task<TEntity> Insert(TEntity entity, bool saveChanges = false)
    {
        var dbEntity = await DbSet.AddAsync(entity);
        if (saveChanges == true)
        {
            await SaveChanges();
        }
        return dbEntity.Entity;
    }
    public virtual async Task<bool> Update(TEntity entity, bool saveChanges = false)
    {
        var entry = DbSet.Attach(entity);
        entry.State = EntityState.Modified;

        if (saveChanges == true) { return await SaveChanges(); }

        return true;
    }
    public virtual async Task<bool> Delete(TEntity entity, bool saveChanges = false)
    {
        DbSet.Attach(entity);
        _context.Remove(entity);
        if (saveChanges == true) { return await SaveChanges(); }

        return true;
    }

    public async Task<bool> SaveChanges()
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
            throw;
        }
    }
    public virtual async Task Dispose()
    {
        await _context.DisposeAsync();
    }

}



//public IQueryable<TEntity> FindAllOrdered<TProperty>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TProperty>> orderBy, bool orderDesc = false)
//{
//    var queryable = DbSet.Where(expression);

//    queryable = orderDesc ? queryable.OrderByDescending(orderBy)
//                          : queryable.OrderBy(orderBy);

//    return queryable;
//}

//public IQueryable<TEntity> FindAllPaged<TProperty>(Expression<Func<TEntity, bool>> expression, int pageNumber, int pageSize)
//{
//    var queryable = DbSet.Where(expression);
//    queryable = queryable.Skip((pageNumber - 1) * pageSize)
//                          .Take(pageSize);

//    return queryable;
//}


//public virtual async Task<TEntity> FindWhere(Expression<Func<TEntity, bool>> expression) => await DbSet.Where(expression).FirstOrDefaultAsync() ?? null!;


//public virtual async Task<IList<TEntity>> FindAllWhere(Expression<Func<TEntity, bool>> expression) => await DbSet.Where(expression).ToListAsync();


//public virtual async Task<IList<TEntity>> FindAllPaginatedWhere<TProperty>(Expression<Func<TEntity, bool>> expression, int pageNumber = 0, int pageSize = 10, Expression<Func<TEntity, TProperty>>? orderBy = null, bool? orderDesc = false)
//{
//    var queryable = DbSet.AsQueryable();
//    queryable = queryable.Where(expression);

//    queryable = queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize);


//    if (orderBy != null) queryable = queryable.OrderBy(orderBy);



//    return await queryable.ToListAsync();

//}

//public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int pageNumber, int pageSize)
//{
//    return queryable
//        .Skip((pageNumber - 1) * pageSize)
//        .Take(pageSize);
//}
//public virtual Task<IList<TEntity>> FindPaginatedWhere<TProperty>(Expression<Func<TEntity, bool>> expression, int pageNumber, int pageSize)
//{
//    throw new NotImplementedException();
//}

//public Task<IQueryable<TEntity>> Order<TProperty>(Expression<Func<TEntity, TProperty>> orderBy, bool? orderDesc)
//{
//    throw new NotImplementedException();
//}

//public IQueryable<TEntity> OrderX<TProperty>(Func<TEntity, TProperty> orderBy, bool? orderDesc)
//{
//    return DbSet.Where(x => x.Id == 5).OrderBy(t => orderBy(t));
//}

//public static IQueryable<TEntity> Paginate(this IQueryable<TEntity> entities, int pageNumber, int pageSize)
//{
//    return entities.Skip((pageNumber - 1) * pageSize)
//                   .Take(pageSize);
//}