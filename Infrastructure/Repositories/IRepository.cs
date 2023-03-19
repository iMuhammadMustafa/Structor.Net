using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Structor.Net.Infrastructure.Entities;

namespace Structor.Net.Infrastructure.Repositories;

public interface IRepository<TEntity> where TEntity : IEntity
{
    IQueryable<TEntity> GetAll();
    IQueryable<TEntity> GetIncluding(params Expression<Func<TEntity, object>>[] included);
    Task<TEntity> GetById(int id);
    Task<TEntity> GetByGuid(Guid guid);
    Task<int> GetCount();
    Task<int> GetCountWhere(Expression<Func<TEntity, bool>> expression);

    Task<TEntity> Insert(TEntity entity, bool saveChanges = false);
    Task<bool> Update(TEntity entity, bool saveChanges = false);
    Task<bool> Delete(TEntity entity, bool saveChanges = false);

    Task<bool> SaveChanges();
    Task Dispose();
}