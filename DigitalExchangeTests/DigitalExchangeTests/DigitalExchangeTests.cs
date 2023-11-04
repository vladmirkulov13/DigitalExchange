using DigitalExchangeService.Extensions;
using DigitalExchangeService.Instruments;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace DigitalExchangeTests;

public class DigitalExchangeTests
{
    private static CurrencyLimited _currencyLimitedDict;
    
    [SetUp]
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        IConfiguration config = builder.Build();
        _currencyLimitedDict= config.GetSection("CurrencyLimited").Get<CurrencyLimited>();
    }

    [Test]
    public void AmountOutOfBordersNullException()
    {
        Assert.DoesNotThrow(() => AmountExtension.AmountOutOfBorders(100, Currency.A, null));
    }
    
    [Test]
    public void AmountOutOfBordersFalse()
    {
        Assert.That(AmountExtension.AmountOutOfBorders(500, Currency.A, _currencyLimitedDict) == false,
            "Некорретное поведение метода AmountOutOfBorders");
    }
    
    [Test]
    public void AmountOutOfBordersTrue()
    {
        Assert.That(AmountExtension.AmountOutOfBorders(66666, Currency.A, _currencyLimitedDict),
            "Некорретное поведение метода AmountOutOfBorders");
    }
    
    [Test]
    public void IsNegativeFalse()
    {
        Assert.That(AmountExtension.NotPositive(1301) == false,
            "Некорретное поведение метода IsNegative");
    }
    
    [Test]
    public void IsNegativeTrue()
    {
        Assert.Multiple(() =>
        {
            Assert.That(AmountExtension.NotPositive(-1301),
                "Некорретное поведение метода IsNegative");
            Assert.That(AmountExtension.NotPositive(0),
                "Некорретное поведение метода IsNegative");
        });
        
    }
}