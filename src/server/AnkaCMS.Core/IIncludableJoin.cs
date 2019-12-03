using System.Linq;

namespace AnkaCMS.Core
{
    public interface IIncludableJoin<out TEntity, out TProperty> : IQueryable<TEntity> where TEntity : class, IEntity, new()
    {
    }
}
