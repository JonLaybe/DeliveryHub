using Catalog.API.Contracts;
using FluentValidation;

namespace Catalog.API.Validators
{
    public class ProductSearchQueryRequestValidator : AbstractValidator<ProductSearchQueryRequest>
    {
        public ProductSearchQueryRequestValidator()
        {
            RuleFor(r => r.Attributes)
                .Must(x => (x?.Count ?? 0) < 100)
                .WithMessage("Превышено максимальное количество атрибутов");

            RuleFor(r => r.CategoryId)
                .NotEqual(Guid.Empty);

            RuleFor(r => r.PageSize)
                .InclusiveBetween(0, 1000);
        }
    }
}
