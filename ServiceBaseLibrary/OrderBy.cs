using System;
using System.Linq;
using System.Linq.Expressions;

namespace ServiceBaseLibrary
{
    public enum OrderDirection
    {
        Ascending,
        Descending
    }

    public interface IOrderBy<TEntity>
    {
        OrderDirection Direction { get; set; }
        LambdaExpression Selector { get; set; }
    }

    public class OrderBy<TEntity> : IOrderBy<TEntity>
    {
        public OrderBy(string entity) : this(entity, OrderDirection.Ascending)
        {
        }

        public OrderBy(string entity, OrderDirection direction)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                Selector = null;
            }
            else
            {
                var parameter = Expression.Parameter(typeof(TEntity), "entity");
                var propertyOrField = entity.Split('.')
                    .Aggregate<string, Expression>(parameter, Expression.PropertyOrField);
                var selector = Expression.Lambda(propertyOrField, parameter);

                Selector = selector;
            }

            Direction = direction;
        }

        public OrderBy(Expression<Func<TEntity, dynamic>> func) : this(func, OrderDirection.Ascending)
        {
        }

        public OrderBy(Expression<Func<TEntity, dynamic>> func, OrderDirection direction)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var unaryExpressionBody = func.Body as UnaryExpression;

            if (unaryExpressionBody == null)
            {
                Selector = func;
            }
            else
            {
                var memberExpression = unaryExpressionBody.Operand as MemberExpression;

                if (memberExpression == null)
                    throw new ArgumentException("Unable to determine operand of expression.", nameof(func));

                Selector = Expression.Lambda(memberExpression, func.Parameters);
            }

            Direction = direction;
        }

        public OrderDirection Direction { get; set; }
        public LambdaExpression Selector { get; set; }
    }
}