using AnkaCMS.Core.Validation;
using System.Collections.Generic;

namespace AnkaCMS.Core
{
    /// <summary>
    /// Doğrulama (validasyon) nesneleri için arayüz
    /// </summary>
    public interface IValidator
    {

        /// <summary>
        /// Doğrulandı mı?
        /// </summary>
        bool IsValid { get; set; }

        /// <summary>
        /// Doğrulama işlemini yapan metod
        /// </summary>
        /// <returns></returns>
        List<ValidationResult> Validate();
    }
}
