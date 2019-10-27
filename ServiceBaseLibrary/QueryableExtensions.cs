using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ServiceBaseLibrary
{
    public static class QueryableExtensions
    {
        /// <summary>
        ///     Extend IQueryable to simplify access to include methods
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="includeProperties"></param>
        /// <returns>IQueryable with Include having been performed</returns>
        public static IQueryable<TEntity> ApplyIncludes<TEntity>(this IQueryable<TEntity> queryable,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            if (includeProperties == null) return queryable;

            return includeProperties.Aggregate(queryable,
                (current, property) => current.Include(property));
        }

        /// <summary>
        ///     Extend IQueryable to simplify access to ordering methods
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="orderExpressions"></param>
        /// <returns>IQueryable with OrderBy having been performed</returns>
        public static IQueryable<TEntity> ApplyOrderBy<TEntity>(this IQueryable<TEntity> queryable,
            IList<IOrderBy<TEntity>> orderExpressions)
        {
            if (orderExpressions == null || !orderExpressions.Any()) return queryable;

            var firstOrderExpression = orderExpressions.First();

            queryable = firstOrderExpression.Direction == OrderDirection.Ascending
                ? Queryable.OrderBy(queryable, firstOrderExpression.Selector as dynamic)
                : Queryable.OrderByDescending(queryable, firstOrderExpression.Selector as dynamic);

            var result = (IOrderedQueryable<TEntity>) queryable;
            return orderExpressions.Skip(1)
                .Aggregate(result,
                    (current, orderExpression) =>
                        orderExpression.Direction == OrderDirection.Ascending
                            ? Queryable.ThenBy(current, orderExpression.Selector as dynamic)
                            : Queryable.ThenByDescending(current, orderExpression.Selector as dynamic));
        }

        /// <summary>
        ///     Extend IQueryable to simplify access to skip and take methods
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="page"></param>
        /// <returns>IQueryable with Skip and Take having been performed</returns>
        public static IQueryable<TEntity> ApplyPaging<TEntity>(this IQueryable<TEntity> queryable, Page<TEntity> page)
        {
            return page == null ? queryable : queryable.Skip(page.NumberOfRecordsToSkip).Take(page.PageSize);
        }
    }
}