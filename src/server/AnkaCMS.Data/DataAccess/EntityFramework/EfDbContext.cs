using AnkaCMS.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace AnkaCMS.Data.DataAccess.EntityFramework
{
    public class EfDbContext : DbContext, IDbContext
    {
        public EfDbContext(DbContextOptions options) : base(options)
        {

        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class => base.Set<TEntity>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(type => !string.IsNullOrEmpty(type.Namespace)))
            {
                for (var i = 0; i < type.GetInterfaces().Count(x => x.IsGenericType); i++)
                {
                    if (!type.FullName.Contains("Configurations")) continue;
                    dynamic configurationInstance = Activator.CreateInstance(type);
                    modelBuilder.ApplyConfiguration(configurationInstance);
                }

            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
