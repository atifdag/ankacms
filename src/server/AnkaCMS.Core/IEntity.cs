using System;

namespace AnkaCMS.Core
{
    /// <summary>
    /// Veri tabanı tabloları için arayüz
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Birincil anahtar
        /// </summary>
        Guid Id { get; set; }

    }
}
