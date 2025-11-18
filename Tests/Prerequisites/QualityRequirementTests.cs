namespace Psyche.Tests.Prerequisites;

using Psyche.Systems.Prerequisites;
using Psyche.Models.Mocks;
using FluentAssertions;
using Xunit;

public class QualityRequirementTests
{
    [Fact]
    public void IsMet_WhenQualityMeetsMinimum_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.Qualities["social_capital"] = 15;

        var requirement = new QualityRequirement
        {
            QualityId = "social_capital",
            MinValue = 10
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_WhenQualityExactlyAtMinimum_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.Qualities["social_capital"] = 10;

        var requirement = new QualityRequirement
        {
            QualityId = "social_capital",
            MinValue = 10
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_WhenQualityBelowMinimum_ReturnsFalse()
    {
        // Arrange
        var character = new Character();
        character.Qualities["social_capital"] = 5;

        var requirement = new QualityRequirement
        {
            QualityId = "social_capital",
            MinValue = 10
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMet_WhenQualityDoesNotExist_UseZeroAsDefault()
    {
        // Arrange
        var character = new Character();
        // social_capital not set, defaults to 0

        var requirement = new QualityRequirement
        {
            QualityId = "social_capital",
            MinValue = 1
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMet_WhenQualityBelowMaximum_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.Qualities["psychological_strain"] = 40;

        var requirement = new QualityRequirement
        {
            QualityId = "psychological_strain",
            MaxValue = 49
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_WhenQualityExactlyAtMaximum_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.Qualities["psychological_strain"] = 49;

        var requirement = new QualityRequirement
        {
            QualityId = "psychological_strain",
            MaxValue = 49
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_WhenQualityAboveMaximum_ReturnsFalse()
    {
        // Arrange
        var character = new Character();
        character.Qualities["psychological_strain"] = 55;

        var requirement = new QualityRequirement
        {
            QualityId = "psychological_strain",
            MaxValue = 49
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMet_WhenQualityInRange_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.Qualities["social_capital"] = 15;

        var requirement = new QualityRequirement
        {
            QualityId = "social_capital",
            MinValue = 10,
            MaxValue = 20
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetDisplayText_WithMinValueOnly_ReturnsCorrectFormat()
    {
        // Arrange
        var requirement = new QualityRequirement
        {
            QualityId = "social_capital",
            MinValue = 10
        };

        // Act
        var result = requirement.GetDisplayText();

        // Assert
        result.Should().Be("social_capital ≥ 10");
    }

    [Fact]
    public void GetDisplayText_WithMaxValueOnly_ReturnsCorrectFormat()
    {
        // Arrange
        var requirement = new QualityRequirement
        {
            QualityId = "psychological_strain",
            MaxValue = 49
        };

        // Act
        var result = requirement.GetDisplayText();

        // Assert
        result.Should().Be("psychological_strain ≤ 49");
    }

    [Fact]
    public void GetDisplayText_WithBothMinAndMax_ReturnsCorrectFormat()
    {
        // Arrange
        var requirement = new QualityRequirement
        {
            QualityId = "social_capital",
            MinValue = 10,
            MaxValue = 20
        };

        // Act
        var result = requirement.GetDisplayText();

        // Assert
        result.Should().Be("social_capital between 10-20");
    }
}
