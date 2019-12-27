using System;
using System.Linq;
using System.Reflection;

namespace AnkaCMS.Core.Helpers
{

    /// <summary>
    /// Mapping işlemleri için yardımcı sınıf. Nesneler arası aktarım yapar.
    /// </summary>
    public static class MappingHelper
    {

        /// <summary>
        /// Kaynak nesnenin verilerini hedef nesneye aktarır. 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            var sourceProperties = typeof(TSource).GetProperties(bindingFlags).Where(propertyInfo => propertyInfo.CanRead).Select(propertyInfo => new
            {
                propertyInfo.Name,
                Type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType
            });

            var targetProperties = target.GetType().GetProperties(bindingFlags).Where(propertyInfo => propertyInfo.CanWrite).Select(propertyInfo => new
            {
                propertyInfo.Name,
                Type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType
            });

            foreach (var property in sourceProperties.Intersect(targetProperties))
            {
                var propertyInfo = source.GetType().GetProperty(property.Name);
                if (propertyInfo == null) continue;
                var value = propertyInfo.GetValue(source, null);
                var propertyInfos = target.GetType().GetProperty(property.Name);
                if (propertyInfos != null) propertyInfos.SetValue(target, value, null);
            }
            return target;
        }

        /// <summary>
        /// Kaynak nesnenin verileri ile istenilen tipte yeni bir nesne oluşturup verileri yeni nesneye aktarır.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget CreateMapped<TSource, TTarget>(this TSource source) where TTarget : new()
        {
            return source.MapTo(new TTarget());
        }
    }
}
