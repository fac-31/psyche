namespace Psyche.Tests.Effects;

using Psyche.Systems.Effects;
using Psyche.Models.Mocks;
using FluentAssertions;
using Xunit;

public class AttributeEffectTests
{
    [Fact]
    public void Apply_WithPositiveDelta_IncreasesAttribute()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 50;

        var effect = new AttributeEffect
        {
            AttributeName = "Bravery",
            Delta = 5
        };

        // Act
        effect.Apply(character);

        // Assert
        character.Attributes.Bravery.Should().Be(55);
    }

    [Fact]
    public void Apply_WithNegativeDelta_DecreasesAttribute()
    {
        // Arrange
        var character = new Character();
        character.Attributes.SelfAssurance = 60;

        var effect = new AttributeEffect
        {
            AttributeName = "SelfAssurance",
            Delta = -10
        };

        // Act
        effect.Apply(character);

        // Assert
        character.Attributes.SelfAssurance.Should().Be(50);
    }

    [Fact]
    public void Apply_WhenExceedingMaximum_ClampsTo100()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 95;

        var effect = new AttributeEffect
        {
            AttributeName = "Bravery",
            Delta = 10 // Would result in 105
        };

        // Act
        effect.Apply(character);

        // Assert
        character.Attributes.Bravery.Should().Be(100);
    }

    [Fact]
    public void Apply_WhenBelowMinimum_ClampsTo0()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Compassion = 5;

        var effect = new AttributeEffect
        {
            AttributeName = "Compassion",
            Delta = -10 // Would result in -5
        };

        // Act
        effect.Apply(character);

        // Assert
        character.Attributes.Compassion.Should().Be(0);
    }

    [Fact]
    public void Apply_MultipleApplications_AffectsAttributeCumulatively()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 50;

        var effect = new AttributeEffect
        {
            AttributeName = "Bravery",
            Delta = 5
        };

        // Act
        effect.Apply(character);
        effect.Apply(character);
        effect.Apply(character);

        // Assert
        character.Attributes.Bravery.Should().Be(65);
    }

    [Fact]
    public void GetDisplayText_WithPositiveDelta_ShowsPlusSign()
    {
        // Arrange
        var effect = new AttributeEffect
        {
            AttributeName = "Bravery",
            Delta = 5
        };

        // Act
        var result = effect.GetDisplayText();

        // Assert
        result.Should().Be("Bravery +5");
    }

    [Fact]
    public void GetDisplayText_WithNegativeDelta_ShowsMinusSign()
    {
        // Arrange
        var effect = new AttributeEffect
        {
            AttributeName = "SelfAssurance",
            Delta = -3
        };

        // Act
        var result = effect.GetDisplayText();

        // Assert
        result.Should().Be("SelfAssurance -3");
    }

    [Fact]
    public void GetDisplayText_WithZeroDelta_ShowsPlusSign()
    {
        // Arrange
        var effect = new AttributeEffect
        {
            AttributeName = "Compassion",
            Delta = 0
        };

        // Act
        var result = effect.GetDisplayText();

        // Assert
        result.Should().Be("Compassion +0");
    }
}
