using System;

namespace AnkaCMS.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Veritabanına karşılık gelen obje için arayüz
    /// </summary>
    public interface IDbContext : IDisposable
    {
        /// <summary>
        /// Değişiklikleri kaydeder. Etkilenen kayıt sayısını döndürür.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}
