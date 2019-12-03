using AnkaCMS.Core.Validation;
using System.Collections.Generic;

namespace AnkaCMS.Core.Exceptions
{


    /// <inheritdoc />
    /// <summary>
    /// Doğrulanmayan kayıtlar için istisna sınıfı
    /// </summary>
    public class ValidationException : BaseApplicationException
    {
        private List<ValidationResult> _validationResult;

        public List<ValidationResult> ValidationResult
        {
            get => _validationResult ?? (_validationResult = new List<ValidationResult>());
            set => _validationResult = value;
        }
        public ValidationException(string message) : base(message) { }
    }
}