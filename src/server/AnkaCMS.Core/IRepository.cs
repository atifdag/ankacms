using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnkaCMS.Core
{
    /// <inheritdoc />
    /// <summary>
    ///  Veri tabanı sorgulama işlemlerinin bir merkezden yapılmasını sağlayan sınıf için arayüz
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> : IDisposable where TEntity : class, IEntity, new()
    {

        /// <summary>
        /// Verileri getirir.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Get();


        IQueryable<TEntity> GetNoTracking();


        IIncludableJoin<TEntity, TProperty> Join<TProperty>(Expression<Func<TEntity, TProperty>> navigationProperty);

        /// <summary>
        /// Async olarak verileri getirir.
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<TEntity>> GetAsync();

        /// <summary>
        /// Sql cümlesi ile veri getirir.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        IQueryable<TEntity> Get(string sql);

        /// <summary>
        /// Async olarak sql cümlesi ile veri getirir.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> GetAsync(string sql);

        /// <summary>
        /// Tek bir veriyi koşula göre getirir.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity Get(Expression<Func<TEntity, bool>> predicate);


        TEntity GetNoTracking(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Async olarak tek bir veriyi koşula göre getirir.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Yeni kayıt ekler. Eklenen kaydı geri döndürür.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        TEntity Add(TEntity entity, bool autoSave = false);

        /// <summary>
        ///  Async olarak yeni kayıt ekler. Eklenen kaydı geri döndürür.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        Task<TEntity> AddAsync(TEntity entity, bool autoSave = false);

        /// <summary>
        /// Veriyi günceller. Güncellenen kaydı geri döndürür.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        TEntity Update(TEntity entity, bool autoSave = false);

        /// <summary>
        /// Async olarak veriyi günceller. Güncellenen kaydı geri döndürür.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false);

        /// <summary>
        /// Veriyi siler
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        void Delete(TEntity entity, bool autoSave = false);

        /// <summary>
        /// Async olarak veriyi siler.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        Task DeleteAsync(TEntity entity, bool autoSave = false);

        /// <summary>
        /// Değişiklikleri kaydeder.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Async olarak değişiklikleri kaydeder.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();

    }
}