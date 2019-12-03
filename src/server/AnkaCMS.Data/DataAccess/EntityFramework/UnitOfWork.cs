using AnkaCMS.Core;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace AnkaCMS.Data.DataAccess.EntityFramework
{
    public class UnitOfWork : IUnitOfWork<EfDbContext>
    {
        private bool _disposed;
        private IDbContextTransaction _transaction;

        public UnitOfWork(EfDbContext context)
        {
            Context = context;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            _disposed = true;
        }
        ~UnitOfWork()
        {
            Dispose(false);
        }

        public void BeginTransaction()
        {
            _transaction = Context.Database.BeginTransaction();
        }

        public void Commit()
        {
            Context.SaveChanges();
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public EfDbContext Context { get; set; }
    }
}
