using Elogbook.Core.Interfaces;
using System;

namespace Elogbook.Data
{
    public class PmisSyncUnitOfWork : IDisposable
    {
        private PmisSyncDataContext _dataContext;

        private bool _disposed;

        public PmisSyncDataContext DataContext
        {
            get
            {
                return _dataContext;
            }
        }

        public PmisSyncUnitOfWork(PmisSyncDataContext context)
        {
            _dataContext = context;
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

        public PmisSyncRepository<TEntity> PmisSyncRepository<TEntity>() where TEntity : class, IPmisSyncEntityBase
        {
            return new PmisSyncRepository<TEntity>(_dataContext);
        }
    }
}
