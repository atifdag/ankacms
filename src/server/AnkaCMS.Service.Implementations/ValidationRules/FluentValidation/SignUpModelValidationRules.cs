using AnkaCMS.Service.Models;
using AnkaCMS.Core.Globalization;
using AnkaCMS.Core.Helpers;
using FluentValidation;

namespace AnkaCMS.Service.Implementations.ValidationRules.FluentValidation
{
    public class SignUpModelValidationRules : AbstractValidator<SignUpModel>
    {
        public SignUpModelValidationRules()
        {
            RuleFor(p => p.IdentityCode).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.IdentityCode));
            RuleFor(p => p.Username).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Username));
            RuleFor(p => p.Username).Length(8, 32).WithMessage(string.Format(Messages.DangerFieldLengthLimit, Dictionary.Username, "8"));
            RuleFor(p => p.Password).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Password));
            RuleFor(p => p.Password).Must(password => password.ValidatePassword()).WithMessage(Messages.DangerInvalidPassword);
            RuleFor(p => p.ConfirmPassword).Equal(p => p.Password).WithMessage(Messages.DangerPasswordsDoNotMatch);
            RuleFor(p => p.Email).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Email));
            RuleFor(p => p.Email).EmailAddress().WithMessage(Messages.DangerInvalidEntitiy);
            RuleFor(p => p.FirstName).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.FirstName));
            RuleFor(p => p.LastName).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.LastName));
        }
    }
}
