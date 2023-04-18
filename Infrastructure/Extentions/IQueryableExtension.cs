using System.Linq.Expressions;

namespace Structor.Infrastructure.Extentions;

public static class IQueryableExtension
{
    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> queryable, int pageNumber, int pageSize)
    {
        return queryable.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize);
    }
    public static IQueryable<TEntity> OrderQueryable<TEntity, TProperty>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, TProperty>> orderBy, bool orderDesc = false)
    {

        queryable = orderDesc ? queryable.OrderByDescending(orderBy)
                              : queryable.OrderBy(orderBy);



        return queryable;
    }
}
