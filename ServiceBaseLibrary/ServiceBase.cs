using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ServiceBaseLibrary
{
    public abstract class ServiceBase<TEntity> : IService<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private protected readonly IDbSet<TEntity> Set;

        protected ServiceBase(DbContext context)
        {
            if (context == null) return;

            _context = context;
            Set = _context.Set<TEntity>();
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }

        public virtual void ClearEntity()
        {
            _context.Set<TEntity>().RemoveRange(Set);
            SaveChanges();
        }

        public virtual int Count()
        {
            return Count(null);
        }

        public virtual int Count(Expression<Func<TEntity, bool>> whereExpression)
        {
            IQueryable<TEntity> resultSet = Set;

            if (whereExpression != null) resultSet = resultSet.Where(whereExpression);

            return resultSet.Count();
        }

        public virtual bool Create(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                Set.Add(entity);
                SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public virtual bool CreateMany(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            try
            {
                foreach (var entity in entities) Set.Add(entity);

                SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public virtual bool Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            try
            {
                var keyNames = GetKeyNames(_context);

                var existing = Set.Find((from keyName in keyNames
                    select typeof(TEntity).GetProperty(keyName)
                    into keyProperty
                    where keyProperty != null
                    select keyProperty.GetValue(entity)).ToArray());

                _context.Entry(existing).CurrentValues.SetValues(entity);

                SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public virtual bool Delete<TValue>(TValue entityId) where TValue : struct
        {
            try
            {
                var entityIds = new[] {entityId};

                return Delete(entityIds);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        ///     Entity ID order must match column order of keys on entity.
        /// </summary>
        /// <param name="entityIds">Order must match column order of keys on entity.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual bool Delete<TValue>(params TValue[] entityIds) where TValue : struct
        {
            try
            {
                var entity = GetByIds(entityIds);
                Set.Remove(entity);
                SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region GetByIds

        public virtual TEntity GetById<TValue>(TValue entityId) where TValue : struct
        {
            var entityIds = new[] {entityId};

            return GetByIds(entityIds);
        }

        public virtual TEntity GetById<TValue>(TValue entityId,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties) where TValue : struct
        {
            var entityIds = new[] {entityId};

            return GetByIds(includeProperties, entityIds);
        }

        /// <summary>
        ///     Entity ID order must match column order of keys on entity.
        /// </summary>
        /// <param name="entityIds">Order must match column order of keys on entity.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual TEntity GetByIds<TValue>(params TValue[] entityIds) where TValue : struct
        {
            return GetByIds(null, entityIds);
        }

        /// <summary>
        ///     Entity ID order must match column order of keys on entity.
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <param name="entityIds">Order must match column order of keys on entity.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual TEntity GetByIds<TValue>(IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties,
            params TValue[] entityIds) where TValue : struct
        {
            var resultSet = GetByIdsQueryable(includeProperties, entityIds);

            return resultSet.FirstOrDefault();
        }

        #endregion

        #region GetByIdsQueryable

        public virtual IQueryable<TEntity> GetByIdQueryable<TValue>(TValue entityId) where TValue : struct
        {
            return GetByIdQueryable(entityId, null);
        }

        public virtual IQueryable<TEntity> GetByIdQueryable<TValue>(TValue entityId,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties) where TValue : struct
        {
            var entityIds = new[] {entityId};

            return GetByIdsQueryable(includeProperties, entityIds);
        }

        /// <summary>
        ///     Entity ID order must match column order of keys on entity.
        /// </summary>
        /// <param name="entityIds">Order must match column order of keys on entity.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetByIdsQueryable<TValue>(params TValue[] entityIds) where TValue : struct
        {
            return GetByIdsQueryable(null, entityIds);
        }

        /// <summary>
        ///     Entity ID order must match column order of keys on entity.
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <param name="entityIds">Order must match column order of keys on entity.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetByIdsQueryable<TValue>(
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties, params TValue[] entityIds)
            where TValue : struct
        {
            var keyNames = GetKeyNames(_context);

            var keyProperties = keyNames.Select(keyName => typeof(TEntity).GetProperty(keyName))
                .Where(keyProperty => keyProperty != null).ToList();

//            if (keyProperties.Count != entityIds.Length) return null;

            var resultSet = Set
                .ApplyIncludes(includeProperties)
                .Where(PropertiesEqual(keyProperties, entityIds));

            return resultSet;
        }

        #endregion

        #region GetSingle

        public virtual TEntity GetSingle()
        {
            return GetSingle(null, null, null);
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> whereExpression)
        {
            return GetSingle(whereExpression, null, null);
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy)
        {
            return GetSingle(whereExpression, orderBy, null);
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> whereExpression,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetSingle(whereExpression, null, includeProperties);
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            var resultSet = GetSingleQueryable(whereExpression, orderBy, includeProperties);

            return resultSet.FirstOrDefault();
        }

        public virtual TEntity GetSingle(IList<IOrderBy<TEntity>> orderBy)
        {
            return GetSingle(null, orderBy, null);
        }

        public virtual TEntity GetSingle(IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetSingle(null, orderBy, includeProperties);
        }

        public virtual TEntity GetSingle(IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetSingle(null, null, includeProperties);
        }

        #endregion

        #region GetSingleQueryable

        public virtual IQueryable<TEntity> GetSingleQueryable()
        {
            return GetSingleQueryable(null, null, null);
        }

        public virtual IQueryable<TEntity> GetSingleQueryable(Expression<Func<TEntity, bool>> whereExpression)
        {
            return GetSingleQueryable(whereExpression, null, null);
        }

        public virtual IQueryable<TEntity> GetSingleQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy)
        {
            return GetSingleQueryable(whereExpression, orderBy, null);
        }

        public virtual IQueryable<TEntity> GetSingleQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetSingleQueryable(whereExpression, null, includeProperties);
        }

        public virtual IQueryable<TEntity> GetSingleQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy, IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            IQueryable<TEntity> resultSet = Set;

            if (whereExpression != null) resultSet = resultSet.Where(whereExpression);

            resultSet = resultSet.ApplyIncludes(includeProperties);

            if (orderBy != null && orderBy.Any()) resultSet = resultSet.ApplyOrderBy(orderBy);

            return resultSet;
        }

        public virtual IQueryable<TEntity> GetSingleQueryable(IList<IOrderBy<TEntity>> orderBy)
        {
            return GetSingleQueryable(null, orderBy, null);
        }

        public virtual IQueryable<TEntity> GetSingleQueryable(IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetSingleQueryable(null, orderBy, includeProperties);
        }

        public virtual IQueryable<TEntity> GetSingleQueryable(
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetSingleQueryable(null, null, includeProperties);
        }

        #endregion

        #region GetMany

        public virtual List<TEntity> GetMany()
        {
            return GetMany(null, null, null);
        }

        public virtual List<TEntity> GetMany(Expression<Func<TEntity, bool>> whereExpression)
        {
            return GetMany(whereExpression, null, null);
        }

        public virtual List<TEntity> GetMany(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy)
        {
            return GetMany(whereExpression, orderBy, null);
        }

        public virtual List<TEntity> GetMany(Expression<Func<TEntity, bool>> whereExpression,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetMany(whereExpression, null, includeProperties);
        }

        public virtual List<TEntity> GetMany(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            var resultSet = GetManyQueryable(whereExpression, orderBy, includeProperties);

            return resultSet.ToList();
        }

        public virtual List<TEntity> GetMany(IList<IOrderBy<TEntity>> orderBy)
        {
            return GetMany(null, orderBy, null);
        }

        public virtual List<TEntity> GetMany(IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetMany(null, orderBy, includeProperties);
        }

        public virtual List<TEntity> GetMany(IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetMany(null, null, includeProperties);
        }

        #endregion

        #region GetManyQueryable

        public virtual IQueryable<TEntity> GetManyQueryable()
        {
            return GetManyQueryable(null, null, null);
        }

        public virtual IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> whereExpression)
        {
            return GetManyQueryable(whereExpression, null, null);
        }

        public virtual IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy)
        {
            return GetManyQueryable(whereExpression, orderBy, null);
        }

        public virtual IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetManyQueryable(whereExpression, null, includeProperties);
        }

        public virtual IQueryable<TEntity> GetManyQueryable(IList<IOrderBy<TEntity>> orderBy)
        {
            return GetManyQueryable(null, orderBy, null);
        }

        public virtual IQueryable<TEntity> GetManyQueryable(IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetManyQueryable(null, orderBy, includeProperties);
        }

        public virtual IQueryable<TEntity> GetManyQueryable(
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetManyQueryable(null, null, includeProperties);
        }

        public virtual IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy, IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            IQueryable<TEntity> resultSet = Set;

            if (whereExpression != null) resultSet = resultSet.Where(whereExpression);

            resultSet = resultSet.ApplyIncludes(includeProperties);

            if (orderBy != null && orderBy.Any()) resultSet = resultSet.ApplyOrderBy(orderBy);

            return resultSet;
        }

        #endregion

        #region GetManyPaged

        public virtual Page<TEntity> GetManyPaged(Page<TEntity> page, Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy)
        {
            return GetManyPaged(page, whereExpression, orderBy, null);
        }

        public virtual Page<TEntity> GetManyPaged(Page<TEntity> page, Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy, IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            if (orderBy == null) throw new ArgumentNullException(nameof(orderBy));
            if (orderBy.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(orderBy), @"There are no OrderBy items in the list.");

            IQueryable<TEntity> resultSet = Set;

            if (whereExpression != null) resultSet = resultSet.Where(whereExpression);

            page.TotalRows = resultSet.Count();

            resultSet = resultSet
                .ApplyIncludes(includeProperties)
                .ApplyOrderBy(orderBy)
                .ApplyPaging(page);

            page = new Page<TEntity>(page.PageNumber, page.PageSize, resultSet.ToList(), page.TotalRows);

            return page;
        }

        public virtual Page<TEntity> GetManyPaged(Page<TEntity> page, IList<IOrderBy<TEntity>> orderBy)
        {
            return GetManyPaged(page, null, orderBy, null);
        }

        public virtual Page<TEntity> GetManyPaged(Page<TEntity> page, IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
        {
            return GetManyPaged(page, null, orderBy, includeProperties);
        }

        #endregion

        #region Helpers

        private static IEnumerable<string> GetKeyNames(IObjectContextAdapter context)
        {
            var set = context.ObjectContext.CreateObjectSet<TEntity>();
            var entitySet = set.EntitySet;
            var keyNames = entitySet.ElementType.KeyMembers.Select(k => k.Name);

            return keyNames;
        }

        private static Expression<Func<TEntity, bool>> PropertiesEqual<TValue>(IList<PropertyInfo> properties,
            IList<TValue> entityIds) where TValue : struct
        {
            var param = Expression.Parameter(typeof(TEntity));
            var bodyExpressions = new List<BinaryExpression>();

            for (var i = 0; i < entityIds.Count; i++)
            {
                var propertyExpression = Expression.Property(param, properties[i]);
                var valueExpression = Expression.Constant(entityIds[i]);
                bodyExpressions.Add(Expression.Equal(propertyExpression, valueExpression));
            }

            if (bodyExpressions.Count == 1) return Expression.Lambda<Func<TEntity, bool>>(bodyExpressions[0], param);

            var body = Expression.AndAlso(bodyExpressions[0], bodyExpressions[1]);

            for (var i = 2; i < entityIds.Count; i++) body = Expression.AndAlso(body, bodyExpressions[i]);

            return Expression.Lambda<Func<TEntity, bool>>(body, param);
        }

        #endregion
    }
}