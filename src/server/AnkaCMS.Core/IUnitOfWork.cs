using System;

namespace AnkaCMS.Core
{

    /// <inheritdoc />
    /// <summary>
    /// Veritabanı ile yapılacak olan tüm işlemleri, tek bir kanal aracılığı ile gerçekleştirme ve hafızada tutma işlemlerini sunmaktadır.
    /// Bu sayede işlemlerin toplu halde gerçekleştirilmesi ve hata durumunda geri alınabilmesi sağlamaktadır.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IUnitOfWork<TContext> : IDisposable where TContext : IDbContext
    {

        /// <summary>
        /// Yeni bir işlem başlatır.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// İşlemleri veritabanına uygular.
        /// </summary>
        void Commit();

        /// <summary>
        /// İşlemleri geri alır.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Veritabanına karşılık gelen obje
        /// </summary>
        TContext Context { get; set; }
    }
}
