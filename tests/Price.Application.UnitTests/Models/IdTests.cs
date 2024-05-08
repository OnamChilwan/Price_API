using FluentAssertions;
using NUnit.Framework;
using Price.Application.Models;

namespace Price.Application.UnitTests.Models;

[TestFixture]
public class IdTests
{
    [Test]
    public void When_Creating_Id_Then_Format_Is_Correct()
    {
        var subject = Id.Create("123456", "REALM", "TERRITORY", "GOLD");
        subject.ToString().Should().Be("realm-territory-gold-123456");
    }
}