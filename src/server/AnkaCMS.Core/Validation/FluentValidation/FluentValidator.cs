using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using AnkaCMS.Core.Helpers;

namespace AnkaCMS.Core.Validation.FluentValidation
{
    /// <inheritdoc />
    /// <summary>
    /// Fluent Validation için doğrulama sınıfı
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TRules"></typeparam>
    public class FluentValidator<TModel, TRules> : IValidator where TModel : class, new() where TRules : AbstractValidator<TModel>, new()
    {

        private readonly TModel _model;
        private TRules _rules;

        public bool IsValid { get; set; }

        protected readonly List<ValidationResult> ValidationResults;

        /// <summary>
        /// Yapıcı metod
        /// </summary>
        /// <param name="model">Doğrulama yapılacak model</param>
        public FluentValidator(TModel model)
        {
            _model = model;
            ValidationResults = new List<ValidationResult>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Doğrulama işlemini yapan metod
        /// </summary>
        /// <returns></returns>
        public List<ValidationResult> Validate()
        {
            _rules = new TRules();
            var results = _rules.Validate(_model);
            foreach (var error in results.Errors)
            {
                ValidationResults.Add(error.CreateMapped<ValidationFailure, ValidationResult>());
            }
            IsValid = results.IsValid;
            return ValidationResults;
        }
    }
}
