using FluentAssertions;
using NUnit.Framework;
using Price.Application.Features;

namespace Price.Application.UnitTests.Features;

[TestFixture]
public class SalesFeatureTests
{
    [Test]
    public void When_Initialising_Sale_Feature_Default_Behaviour_Then_Default_Is_False()
    {
        var subject = SalesFeature.Default();
        subject.Enabled.Should().BeFalse();
    }
}