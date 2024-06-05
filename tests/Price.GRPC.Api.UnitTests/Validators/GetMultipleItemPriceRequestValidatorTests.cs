using Price.GRPC.Api.Validators;

namespace Price.GRPC.Api.UnitTests.Validators;

public class GetMultipleItemPriceRequestValidatorTests
{
    [TestCase("", "", "")]
    [TestCase(" ", " ", " ")]
    public async Task Given_Invalid_Request_Then_Validation_Fails(string territory, string realm, string language)
    {
        var subject = new GetMultipleItemPriceRequestValidator();
        var result = await subject.ValidateAsync(CreateRequest(territory, realm, language));
        
        Assert.That(result.IsValid, Is.False);
    }

   [TestCase("")]
   [TestCase(" ")]
    public async Task Given_Invalid_Realm_Then_Validation_Fails(string realm)
    {
        var subject = new GetMultipleItemPriceRequestValidator();
        var result = await subject.ValidateAsync(CreateRequest(realm: realm));

        Assert.That(result.IsValid, Is.False);
    }
    
    [TestCase("")]
    [TestCase(" ")]
    public async Task Given_Invalid_Language_Then_Validation_Fails(string language)
    {
        var subject = new GetMultipleItemPriceRequestValidator();
        var result = await subject.ValidateAsync(CreateRequest(language: language));

        Assert.That(result.IsValid, Is.False);
    }
    
    [TestCase("")]
    [TestCase(" ")]
    public async Task Given_Invalid_Territory_Then_Validation_Fails(string territory)
    {
        var subject = new GetMultipleItemPriceRequestValidator();
        var result = await subject.ValidateAsync(CreateRequest(territory: territory));

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task Given_Invalid_Item_Numbers_Then_Validation_Fails()
    {
        var subject = new GetMultipleItemPriceRequestValidator();
        var result = await subject.ValidateAsync(CreateRequest(itemNumbers: []));

        Assert.That(result.IsValid, Is.False);
    }

    private static GetMultipleItemPriceRequest CreateRequest(
        string territory = "territory",
        string realm = "realm",
        string language = "lang")
    {
        return CreateRequest(new[] { "123" }, territory, realm, language);
    }

    private static GetMultipleItemPriceRequest CreateRequest(
        IEnumerable<string> itemNumbers,
        string territory = "territory",
        string realm = "realm",
        string language = "lang")
    {
        return new GetMultipleItemPriceRequest
        {
            Territory = territory,
            ItemNumber = { itemNumbers },
            Realm = realm,
            Language = language
        };
    }
}