using AnkaCMS.Core;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AnkaCMS.Data.DataAccess.EntityFramework
{
    public class IncludableJoin<TEntity, TPreviousProperty> : IIncludableJoin<TEntity, TPreviousProperty> where TEntity : class, IEntity, new()
    {
        private readonly IIncludableQueryable<TEntity, TPreviousProperty> _query;

        public IncludableJoin(IIncludableQueryable<TEntity, TPreviousProperty> query)
        {
            _query = query;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _query.GetEnumerator();
        }

        public Expression Expression => _query.Expression;
        public Type ElementType => _query.ElementType;
        public IQueryProvider Provider => _query.Provider;

        internal IIncludableQueryable<TEntity, TPreviousProperty> GetQuery()
        {
            return _query;
        }
    }
}
