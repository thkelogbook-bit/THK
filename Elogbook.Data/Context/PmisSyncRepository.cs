using Elogbook.Core.Interfaces;
using Elogbook.Core.Interfaces.Repository;
using EntityFramework.DynamicFilters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Elogbook.Core.Constants.CommonConstant;

namespace Elogbook.Data
{
    public class PmisSyncRepository<T> : IRepository<T>
        where T : class, IPmisSyncEntityBase
    {
        #region Protected Fields

        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        #endregion Protected Fields

        #region Constructor

        public PmisSyncRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        #endregion Constructor

        #region Public Virtual Methods

        #region Add Entity

        /// <summary>
        /// This function to add entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// This function to add entities
        /// </summary>
        /// <param name="entities"></param>
        public virtual void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        /// <summary>
        /// This function detect to add/update entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void AddOrUpdate(T entity)
        {
            throw new NotImplementedException();
        }

        #endregion Add Entity

        #region Update Entity

        /// <summary>
        /// This function update entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
            UpdateEntityObject(entity);
        }

        /// <summary>
        /// This function update entities
        /// </summary>
        /// <param name="entities"></param>
        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                UpdateEntityObject(entity);
            }
        }

        #endregion

        #region Delete Entity

        /// <summary>
        /// This function remove or update entity
        /// If isHardDelete = true then remove entity from DB, else update flag IsDeleted = true for entity on DB
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="userID">Current User</param>
        /// <param name="isHardDelete">is hard delete?</param>
        public virtual void Delete(T entity, int userID, bool isHardDelete = false)
        {
            if (isHardDelete)
            {
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                    _dbSet.Attach(entity);
                _dbSet.Remove(entity);
            }
            else
            {
                UpdateEntityObject(entity);
            }
        }

        /// <summary>
        /// This function remove or update entities
        /// If isHardDelete = true then remove entity from DB, else update flag IsDeleted = true for entity on DB
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="isHardDelete">is hard delete?</param>
        public virtual void Delete(Expression<Func<T, bool>> where, int userID, bool isHardDelete = false)
        {
            var entities = GetQuery(where).AsEnumerable();
            foreach (var entity in entities)
            {
                Delete(entity, userID, isHardDelete);
            }
        }

        /// <summary>
        /// This function remove or update entities
        /// If isHardDelete = true then remove entity from DB, else update flag IsDeleted = true for entity on DB
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="isHardDelete"></param>
        public virtual void Delete(IEnumerable<T> entities, int userID, bool isHardDelete = false)
        {
            foreach (var entity in entities)
            {
                Delete(entity, userID, isHardDelete);
            }
        }

        #endregion Delete Entity

        #region Get Entities

        /// <summary>
        /// This function get entity by id and return entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        /// This function get entity by id and return entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<T> GetByIdAsync(int id)
        {
            return _dbSet.FindAsync(id);
        }

        /// <summary>
        /// This function get entity by id and return entities
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetQueryById(int id)
        {
            return GetQuery();
        }

        /// <summary>
        /// This function get entity by id and return entity
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="id"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual Task<TResult> GetPropertyById<TResult>(int id, Expression<Func<T, TResult>> selector)
        {
            return GetQueryById(id).Select(selector).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This function get entities
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetQuery()
        {
            return _dbSet.AsQueryable();
        }

        /// <summary>
        /// This function get entities by where condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> where)
        {
            return GetQuery().Where(where).AsNoTracking();
        }

        /// <summary>
        /// This function get entities by where condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual Task<List<T>> GetList(Expression<Func<T, bool>> where)
        {
            return GetQuery().Where(where).ToListAsync();
        }

        /// <summary>
        /// This function get entities has flag IsDeleted = false
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetQueryWithDeleted()
        {
            _dbContext.DisableFilter(EntityFrameworkConfiguration.FilterIsDelete);
            return GetQuery();
        }

        /// <summary>
        /// This function get entities by filter condition
        /// </summary>
        /// <param name="fieldvalues"></param>
        /// <param name="fieldNames"></param>
        /// <param name="IsActive"></param>
        /// <returns></returns>
        public virtual IQueryable<T> SearchByQuery(List<string> fieldvalues, List<string> fieldNames, bool? IsActive)
        {
            return Enumerable.Empty<T>().AsQueryable();
        }

        #endregion Get Entities

        #region Execute SQL Query

        /// <summary>
        /// This function execute sql job
        /// </summary>
        /// <param name="query"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public virtual IQueryable<T> SqlQuery(string query, string jobName)
        {
            var latParam = new ObjectParameter("@job_name", jobName);

            object[] parameters = { latParam };

            return _dbSet.SqlQuery(query, parameters).AsQueryable();
        }

        /// <summary>
        /// This function execute sql procedure and return entities
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IQueryable<T> SqlQuery(string query, params SqlParameter[] parameters)
        {
            return _dbSet.SqlQuery(query, parameters).AsQueryable();
        }

        /// <summary>
        /// This function execute sql procedure and return entities
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IQueryable<TResult> SqlQuery<TResult>(string query, params SqlParameter[] parameters)
        {
            return _dbContext.Database.SqlQuery<TResult>(query, parameters).AsQueryable();
        }

        #endregion Execute SQL Query

        public virtual T Refresh(T entity)
        {
            return null;
        }

        public virtual void Save(T entity)
        {
        }

        public virtual void Detach(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }
        }

        #endregion Public Virtual Methods

        #region Private Methods

        private void UpdateEntityObject(T entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        #endregion Private Methods
    }
}
