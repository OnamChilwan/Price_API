using Price.Application.DTOs;
using Price.Application.Models;

namespace Price.Application.Decorators;

public interface IDecorator
{
    Task<IEnumerable<ItemPriceDto>> Decorate(DecoratorContext context);
}