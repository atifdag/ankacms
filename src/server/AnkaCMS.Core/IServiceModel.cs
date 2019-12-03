using AnkaCMS.Core.ValueObjects;
using System;

namespace AnkaCMS.Core
{

    /// <summary>
    /// Servis işlemlerinde kullanılacak modeller için temel arayüz
    /// </summary>
    public interface IServiceModel
    {
        /// <summary>
        /// Birincil anahtar
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Sıra No
        /// </summary>
        int DisplayOrder { get; set; }

        /// <summary>
        /// Onaylı kayıt mı?
        /// </summary>
        bool IsApproved { get; set; }

        /// <summary>
        /// Sürüm No
        /// </summary>
        int Version { get; set; }


        /// <summary>
        /// İlk oluşturulma zamanı
        /// </summary>
        DateTime CreationTime { get; set; }

        /// <summary>
        /// İlk oluşturan kullanıcının id'si ve tam adı
        /// </summary>
        IdCodeName Creator { get; set; }

        /// <summary>
        /// Son değişiklik zamanı
        /// </summary>
        DateTime LastModificationTime { get; set; }

        /// <summary>
        /// Son güncelleme işlemi yapan kullanıcının id'si ve tam adı
        /// </summary>
        IdCodeName LastModifier { get; set; }

    }
}
