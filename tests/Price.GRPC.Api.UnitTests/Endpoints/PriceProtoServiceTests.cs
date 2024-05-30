using Grpc.Core;
using NSubstitute;
using Price.Application.Decorators;
using Price.Application.Services;
using Price.GRPC.Api.Endpoints;
using Price.GRPC.Api.Validators;
using Price.Infrastructure.Queries;

namespace Price.GRPC.Api.UnitTests.Endpoints;

public class PriceProtoServiceTests
{
    private PriceProtoService _subject = null!;
    private IDecorator _decorator = null!;
    private IGetMultiplePricesQuery _query = null!;
    private PriceApplicationService _priceApplicationService = null!;

    [SetUp]
    public void Setup()
    {
        var validator = new GetMultipleItemPriceRequestValidator();
        
        _query = Substitute.For<IGetMultiplePricesQuery>();
        _decorator = Substitute.For<IDecorator>();
        _priceApplicationService = new PriceApplicationService(_query, _decorator);
        _subject = new PriceProtoService(validator, _priceApplicationService);
    }

    [Test]
    public async Task Test1()
    {
        var stream = Substitute.For<IServerStreamWriter<ItemPrice>>();
        var request = new GetMultipleItemPriceRequest
        {
            ItemNumber = { "123", "456" },
            Realm = "next",
            Territory = "gb",
            Language = "en"
        };
        
        await _subject.Get(request, stream, Substitute.For<ServerCallContext>());

        await _decorator.Received(1).Decorate(Arg.Any<DecoratorContext>());
    }
}