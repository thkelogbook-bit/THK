using Elogbook.Core.Interfaces;
using System;

namespace Elogbook.Data
{
    public class PmisUnitOfWork : IDisposable
    {
        private PmisDataContext _dataContext;

        private bool _disposed;

        public PmisDataContext DataContext
        {
            get
            {
                return _dataContext;
            }
        }

        public PmisUnitOfWork(PmisDataContext context)
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

        public PmisRepository<TEntity> PmisRepository<TEntity>() where TEntity : class, IPmisEntityBase
        {
            return new PmisRepository<TEntity>(_dataContext);
        }
    }
}
