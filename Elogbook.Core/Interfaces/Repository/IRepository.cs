using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Elogbook.Core.Interfaces.Repository
{
    /// <summary>
    /// Define methods working with entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);

        /// <summary>
        /// Add multiple entities
        /// </summary>
        /// <param name="entities"></param>
        void AddRange(IEnumerable<T> entities);

        /// <summary>
        /// Add or upate entity
        /// </summary>
        /// <param name="entity"></param>
        void AddOrUpdate(T entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// Update multiple entities
        /// </summary>
        /// <param name="entities"></param>
        void Update(IEnumerable<T> entities);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userID"></param>
        /// <param name="isHardDelete"></param>
        void Delete(T entity, int userID, bool isHardDelete = false);

        /// <summary>
        /// Delete entities by condition
        /// </summary>
        /// <param name="where"></param>
        /// <param name="userID"></param>
        /// <param name="isHardDelete"></param>
        void Delete(Expression<Func<T, bool>> where, int userID, bool isHardDelete = false);

        /// <summary>
        /// Delete multiple entities
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="userID"></param>
        /// <param name="isHardDelete"></param>
        void Delete(IEnumerable<T> entities, int userID, bool isHardDelete = false);

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(int id);

        /// <summary>
        /// Get entity async by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Get list entities by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<T> GetQueryById(int id);

        /// <summary>
        /// Get list properties by id
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="id"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<TResult> GetPropertyById<TResult>(int id, Expression<Func<T, TResult>> selector);

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetQuery();

        /// <summary>
        /// Get entities by condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IQueryable<T> GetQuery(Expression<Func<T, bool>> where);

        /// <summary>
        /// Get list by codition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<List<T>> GetList(Expression<Func<T, bool>> where);

        /// <summary>
        /// Get list with deleted
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetQueryWithDeleted();

        /// <summary>
        /// Get list by query
        /// </summary>
        /// <param name="fieldvalues"></param>
        /// <param name="fieldNames"></param>
        /// <param name="IsActive"></param>
        /// <returns></returns>
        IQueryable<T> SearchByQuery(List<string> fieldvalues, List<string> fieldNames, bool? IsActive);

        /// <summary>
        /// Execute sql query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        IQueryable<T> SqlQuery(string query, string jobName);

        /// <summary>
        /// Execute sql query with condition
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IQueryable<T> SqlQuery(string query, params SqlParameter[] parameters);

        /// <summary>
        /// Execute list by sql query
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IQueryable<TResult> SqlQuery<TResult>(string query, params SqlParameter[] parameters);

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="entity"></param>
        void Save(T entity);

        /// <summary>
        /// Refresh entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Refresh(T entity);
    }
}
