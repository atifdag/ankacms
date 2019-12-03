using AnkaCMS.Service.Models;
using AnkaCMS.Core.Globalization;
using FluentValidation;

namespace AnkaCMS.Service.Implementations.ValidationRules.FluentValidation
{
    public class ParameterValidationRules : AbstractValidator<ParameterModel>
    {
        public ParameterValidationRules()
        {
            RuleFor(p => p.Key).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Key));
            RuleFor(p => p.Value).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Value));
        }
    }
}
