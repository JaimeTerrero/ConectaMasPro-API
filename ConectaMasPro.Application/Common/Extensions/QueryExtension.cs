using System.Linq.Expressions;

namespace ConectaMasPro.Application.Common.Extensions
{
    public static class QueryExtension
    {
        public static IQueryable<TEntity> WithCondition<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate, bool condition = false)
        {
            if (condition)
                query = query.Where(predicate);

            return query;
        }

        public static IQueryable<TEntity> WithOrderBy<TEntity, TKey>(
            this IQueryable<TEntity> query,
            Expression<Func<TEntity, TKey>> key,
            bool condition = false,
            string type = "asc")
        {
            if (!condition)
                return query;

            return type.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(key)
                : query.OrderBy(key);
        }

        public static IQueryable<TEntity> WithOrderByWithType<TEntity, TKey>(this IQueryable<TEntity> query, Expression<Func<TEntity, TKey>> key, bool condition = false, string type = "asc")
        {
            if (condition)
            {
                if (type == "desc")
                    return query.OrderByDescending(key);
                else return query.OrderBy(key);
            }
            return query;

        }
    }
}
