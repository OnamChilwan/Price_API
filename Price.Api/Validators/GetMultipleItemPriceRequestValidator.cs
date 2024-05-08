using FluentValidation;
using Price.Api.Models.Requests;

namespace Price.Api.Validators;

public class GetMultipleItemPriceRequestValidator : AbstractValidator<GetMultipleItemPriceRequest>
{
    public GetMultipleItemPriceRequestValidator()
    {
        RuleFor(x => x.Language).NotNull();
        RuleFor(x => x.Realm).NotNull();
        RuleFor(x => x.Territory).NotNull();
        RuleFor(x => x.ItemNumber).NotEmpty();
    }
}