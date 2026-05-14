using Chat.Application.DTOs;
using FluentValidation;

namespace Chat.Application.Validators
{
    public class CreateConversationRequestValidator : AbstractValidator<CreateConversationRequest>
    {
        private static readonly string ForbiddenGuid = "00000000-0000-0000-0000-000000000000";

        public CreateConversationRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId обязателен")
                .Must(x => x != ForbiddenGuid).WithMessage($"ProductId не может быть {ForbiddenGuid}")
                .Must(IsValidGuid).WithMessage("ProductId должен быть корректным GUID");

            RuleFor(x => x.SellerId)
                .NotEmpty().WithMessage("SellerId обязателен")
                .Must(x => x != ForbiddenGuid).WithMessage($"SellerId не может быть {ForbiddenGuid}")
                .Must(IsValidGuid).WithMessage("SellerId должен быть корректным GUID");
        }

        private bool IsValidGuid(string guid)
        {
            return Guid.TryParse(guid, out _);
        }
    }
}
