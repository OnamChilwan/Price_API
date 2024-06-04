using FluentValidation;

namespace Price.GRPC.Api.Validators;

public class GetMultipleItemPriceRequestValidator : AbstractValidator<GetMultipleItemPriceRequest>
{
    public GetMultipleItemPriceRequestValidator()
    {
        RuleFor(x => x.Language)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Realm)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Territory)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.ItemNumber)
            .NotNull()
            .NotEmpty();
    }
}