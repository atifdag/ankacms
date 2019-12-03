using AnkaCMS.Service.Models;
using AnkaCMS.Core.Globalization;
using FluentValidation;

namespace AnkaCMS.Service.Implementations.ValidationRules.FluentValidation
{
    public class LanguageValidationRules : AbstractValidator<LanguageModel>
    {
        public LanguageValidationRules()
        {
            RuleFor(p => p.Code).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Code));
            RuleFor(p => p.Name).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Name));
        }
    }
}
