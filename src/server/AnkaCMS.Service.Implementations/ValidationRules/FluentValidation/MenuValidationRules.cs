using AnkaCMS.Service.Models;
using AnkaCMS.Core.Globalization;
using FluentValidation;

namespace AnkaCMS.Service.Implementations.ValidationRules.FluentValidation
{
    public class MenuValidationRules : AbstractValidator<MenuModel>
    {
        public MenuValidationRules()
        {
            RuleFor(p => p.Code).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Code));
            RuleFor(p => p.Name).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Name));
            RuleFor(p => p.Address).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Address));
        }
    }
}
