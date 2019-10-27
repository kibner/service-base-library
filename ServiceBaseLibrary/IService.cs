using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ServiceBaseLibrary
{
    public interface IService<TEntity> where TEntity : class
    {
        int Count();
        int Count(Expression<Func<TEntity, bool>> whereExpression);
        bool Create(TEntity entity);
        bool CreateMany(IEnumerable<TEntity> entities);
        bool Update(TEntity entity);
        bool Delete<TValue>(TValue entityId) where TValue : struct;
        bool Delete<TValue>(params TValue[] entityIds) where TValue : struct;
        void ClearEntity();

        #region GetById/s

        TEntity GetById<TValue>(TValue entityId) where TValue : struct;

        TEntity GetById<TValue>(TValue entityId, IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties)
            where TValue : struct;

        TEntity GetByIds<TValue>(params TValue[] entityIds) where TValue : struct;

        TEntity GetByIds<TValue>(IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties,
            params TValue[] entityIds) where TValue : struct;

        #endregion

        #region GetById/sQueryable

        IQueryable<TEntity> GetByIdQueryable<TValue>(TValue entityId) where TValue : struct;

        IQueryable<TEntity> GetByIdQueryable<TValue>(TValue entityId,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties) where TValue : struct;

        IQueryable<TEntity> GetByIdsQueryable<TValue>(params TValue[] entityIds) where TValue : struct;

        IQueryable<TEntity> GetByIdsQueryable<TValue>(IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties,
            params TValue[] entityIds) where TValue : struct;

        #endregion

        #region GetSingle

        TEntity GetSingle();

        TEntity GetSingle(Expression<Func<TEntity, bool>> whereExpression);

        TEntity GetSingle(Expression<Func<TEntity, bool>> whereExpression, IList<IOrderBy<TEntity>> orderBy);

        TEntity GetSingle(Expression<Func<TEntity, bool>> whereExpression,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        TEntity GetSingle(Expression<Func<TEntity, bool>> whereExpression, IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        TEntity GetSingle(IList<IOrderBy<TEntity>> orderBy);

        TEntity GetSingle(IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        TEntity GetSingle(IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        #endregion

        #region GetSingleQueryable

        IQueryable<TEntity> GetSingleQueryable();

        IQueryable<TEntity> GetSingleQueryable(Expression<Func<TEntity, bool>> whereExpression);

        IQueryable<TEntity> GetSingleQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy);

        IQueryable<TEntity> GetSingleQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        IQueryable<TEntity> GetSingleQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy, IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        IQueryable<TEntity> GetSingleQueryable(IList<IOrderBy<TEntity>> orderBy);

        IQueryable<TEntity> GetSingleQueryable(IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        IQueryable<TEntity> GetSingleQueryable(IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        #endregion

        #region GetMany

        List<TEntity> GetMany();

        List<TEntity> GetMany(Expression<Func<TEntity, bool>> whereExpression);

        List<TEntity> GetMany(Expression<Func<TEntity, bool>> whereExpression, IList<IOrderBy<TEntity>> orderBy);

        List<TEntity> GetMany(Expression<Func<TEntity, bool>> whereExpression,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        List<TEntity> GetMany(Expression<Func<TEntity, bool>> whereExpression, IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        List<TEntity> GetMany(IList<IOrderBy<TEntity>> orderBy);

        List<TEntity> GetMany(IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        List<TEntity> GetMany(IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        #endregion

        #region GetManyQueryable

        IQueryable<TEntity> GetManyQueryable();

        IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> whereExpression);

        IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy);

        IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy, IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        IQueryable<TEntity> GetManyQueryable(IList<IOrderBy<TEntity>> orderBy);

        IQueryable<TEntity> GetManyQueryable(IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        IQueryable<TEntity> GetManyQueryable(IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        #endregion

        #region GetManyPaged

        Page<TEntity> GetManyPaged(Page<TEntity> page, Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy);

        Page<TEntity> GetManyPaged(Page<TEntity> page, Expression<Func<TEntity, bool>> whereExpression,
            IList<IOrderBy<TEntity>> orderBy, IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        Page<TEntity> GetManyPaged(Page<TEntity> page, IList<IOrderBy<TEntity>> orderBy);

        Page<TEntity> GetManyPaged(Page<TEntity> page, IList<IOrderBy<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, dynamic>>> includeProperties);

        #endregion
    }
}