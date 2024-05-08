using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestStack.BDDfy;

namespace Price.Api.ComponentTests;

// TODO: Placeholder
[Story(
    AsA = "As an Account Holder",
    IWant = "I want to withdraw cash from an ATM",
    SoThat = "So that I can get money when the bank is closed")]
public class Tests
{
    private readonly MultipleItemPriceSteps _steps;

    public Tests()
    {
        _steps = new MultipleItemPriceSteps();
    }

    [Test]
    public void Checker()
    {
        Subject subject = null;

        if (subject == null)
        {
            Console.WriteLine("A");
        }

        if (subject is null)
        {
            Console.WriteLine("B");
        }
        
        // if (subject.Equals(null))
        // {
        //     Console.WriteLine("B");
        // }

        var f = new Foo("afa");
    }
    
    // [Test]
    // public void MyTest()
    // {
    //     this.Given(s => s.GivenSalesFeatureIsOff())
    //         .And(s => s.FollowingItemsExist("", ""))
    //         .When(s => s.ValidRequestIsSendToMultiplePrices())
    //         .Then(s => s.CorrectHttpResponseCodeIsReturned())
    //         .And(s => s.MultipleItemPricesAreReturned())
    //         .BDDfy();
    // }
}

public record Foo(string Id)
{
}

public class Subject
{
    public override bool Equals(object? obj)
    {
        var s = "afsaf";
        return base.Equals(obj);
    }

    public static bool operator !=(Subject left, Subject right)
    {
        return false;
    }

    public static bool operator ==(Subject left, Subject right)
    {
        return !(left != right);
    }
}

public class MultipleItemPriceSteps
{
}

public class TestStartup : Startup
{
    public TestStartup() : base(BuildConfiguration())
    {
    }
    
    protected override void ConfigureExternalDependencies(IServiceCollection services)
    {
        //services.AddTransient(_ => new CosmosContainerFactory();
        //services.AddTransient<IGetMultiplePricesQuery, CosmosGetMultiplePricesQuery>();
    }
    
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
    }
}