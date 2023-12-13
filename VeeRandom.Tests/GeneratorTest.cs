using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeeRandomGenerator;

namespace VeeRandom.Tests;

public class GeneratorTest
{
    [Fact]
    public void GenerateRandomNumber_ShouldReturnInitialValueFirstTime()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<GeneratorSettings>>();
        optionsMock.Setup(x => x.Value).Returns(new GeneratorSettings
        {
            Modulus = 100,
            Multiplier = 23,
            Increment = 7,
            InitialValue = 42
        });

        var generator = new Generator(optionsMock.Object);

        // Act
        var result = generator.GenerateRandomNumber();

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void GenerateRandomNumber_ShouldProduceDifferentValuesAfterFirstCall()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<GeneratorSettings>>();
        optionsMock.Setup(x => x.Value).Returns(new GeneratorSettings
        {
            Modulus = 100,
            Multiplier = 23,
            Increment = 7,
            InitialValue = 42
        });

        var generator = new Generator(optionsMock.Object);

        // Act
        var result1 = generator.GenerateRandomNumber();
        var result2 = generator.GenerateRandomNumber();

        // Assert
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void GetPeriod_ShouldReturnCorrectPeriod()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<GeneratorSettings>>();
        optionsMock.Setup(x => x.Value).Returns(new GeneratorSettings
        {
            Modulus = 67108863,
            Multiplier = 2197,
            Increment = 1597,
            InitialValue = 86
        });

        var generator = new Generator(optionsMock.Object);

        // Act
        var period = generator.GetPeriod();

        // Assert
        Assert.Equal(1365, period);
    }
}
