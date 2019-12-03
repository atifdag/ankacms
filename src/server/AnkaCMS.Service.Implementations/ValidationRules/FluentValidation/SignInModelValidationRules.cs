using AnkaCMS.Service.Models;
using AnkaCMS.Core.Globalization;
using FluentValidation;

namespace AnkaCMS.Service.Implementations.ValidationRules.FluentValidation
{
    public class SignInModelValidationRules : AbstractValidator<SignInModel>
    {
        public SignInModelValidationRules()
        {
            RuleFor(p => p.Username).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Username));
            RuleFor(p => p.Password).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Password));
            RuleFor(p => p.Key).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Key));
        }
    }
}
