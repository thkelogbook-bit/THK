using Elogbook.Core.Interfaces;
using System;

namespace Elogbook.Data
{
    public class ElogbookUnitOfWork : IDisposable
    {
        private ElogbookDataContext _dataContext;

        private bool _disposed;

        public ElogbookDataContext DataContext
        {
            get
            {
                return _dataContext;
            }
        }

        public ElogbookUnitOfWork(ElogbookDataContext context)
        {
            this._dataContext = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing && _dataContext != null)
            {
                _dataContext.Dispose();
                _dataContext = null;
            }

            _disposed = true;
        }

        public int Complete()
        {
            return _dataContext.SaveChanges();
        }

        public ElogbookRepository<TEntity> ElogbookRepository<TEntity>() where TEntity : class, IEntityBase
        {
            return new ElogbookRepository<TEntity>(_dataContext);
        }
    }
}
