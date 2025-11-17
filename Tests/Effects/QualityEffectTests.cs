namespace Psyche.Tests.Effects;

using Psyche.Systems.Effects;
using Psyche.Models.Mocks;
using FluentAssertions;
using Xunit;

public class QualityEffectTests
{
    [Fact]
    public void Apply_WithPositiveDelta_IncreasesQuality()
    {
        // Arrange
        var character = new Character();
        character.Qualities["social_capital"] = 10;

        var effect = new QualityEffect
        {
            QualityId = "social_capital",
            Delta = 5
        };

        // Act
        effect.Apply(character);

        // Assert
        character.GetQualityValue("social_capital").Should().Be(15);
    }

    [Fact]
    public void Apply_WithNegativeDelta_DecreasesQuality()
    {
        // Arrange
        var character = new Character();
        character.Qualities["psychological_strain"] = 50;

        var effect = new QualityEffect
        {
            QualityId = "psychological_strain",
            Delta = -10
        };

        // Act
        effect.Apply(character);

        // Assert
        character.GetQualityValue("psychological_strain").Should().Be(40);
    }

    [Fact]
    public void Apply_WhenQualityDoesNotExist_CreatesIt()
    {
        // Arrange
        var character = new Character();
        // main_story_progress not yet set

        var effect = new QualityEffect
        {
            QualityId = "main_story_progress",
            Delta = 1
        };

        // Act
        effect.Apply(character);

        // Assert
        character.GetQualityValue("main_story_progress").Should().Be(1);
    }

    [Fact]
    public void Apply_CanResultInNegativeValues()
    {
        // Arrange
        var character = new Character();
        character.Qualities["enemies_made"] = 2;

        var effect = new QualityEffect
        {
            QualityId = "enemies_made",
            Delta = -5
        };

        // Act
        effect.Apply(character);

        // Assert
        character.GetQualityValue("enemies_made").Should().Be(-3);
    }

    [Fact]
    public void Apply_MultipleApplications_AffectsQualityCumulatively()
    {
        // Arrange
        var character = new Character();
        character.Qualities["social_capital"] = 10;

        var effect = new QualityEffect
        {
            QualityId = "social_capital",
            Delta = 5
        };

        // Act
        effect.Apply(character);
        effect.Apply(character);
        effect.Apply(character);

        // Assert
        character.GetQualityValue("social_capital").Should().Be(25);
    }

    [Fact]
    public void GetDisplayText_WithPositiveDelta_ShowsPlusSign()
    {
        // Arrange
        var effect = new QualityEffect
        {
            QualityId = "main_story_progress",
            Delta = 1
        };

        // Act
        var result = effect.GetDisplayText();

        // Assert
        result.Should().Be("main_story_progress +1");
    }

    [Fact]
    public void GetDisplayText_WithNegativeDelta_ShowsMinusSign()
    {
        // Arrange
        var effect = new QualityEffect
        {
            QualityId = "psychological_strain",
            Delta = -10
        };

        // Act
        var result = effect.GetDisplayText();

        // Assert
        result.Should().Be("psychological_strain -10");
    }
}
