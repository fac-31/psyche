namespace Psyche.Tests.Prerequisites;

using Psyche.Systems.Prerequisites;
using Psyche.Models.Mocks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

public class AttributeRequirementTests
{
    private readonly ITestOutputHelper _output;

    public AttributeRequirementTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void IsMet_WhenValueMeetsMinimum_ReturnsTrue()
    {
        // Arrange
        TestOutputHelpers.LogScenario(_output, "Brave Character Meets Bravery Requirement");

        var character = new Character();
        character.Attributes.Bravery = 65;

        var requirement = new AttributeRequirement
        {
            AttributeName = "Bravery",
            MinValue = 60
        };

        TestOutputHelpers.LogCharacterState(_output, character, "Initial State");

        // Act
        var result = requirement.IsMet(character);

        // Assert
        TestOutputHelpers.LogPrerequisiteCheck(_output, requirement, character);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_WhenValueExactlyAtMinimum_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 60;

        var requirement = new AttributeRequirement
        {
            AttributeName = "Bravery",
            MinValue = 60
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_WhenValueBelowMinimum_ReturnsFalse()
    {
        // Arrange
        TestOutputHelpers.LogScenario(_output, "Cautious Character Fails Bravery Requirement");

        var character = new Character();
        character.Attributes.Bravery = 55;

        var requirement = new AttributeRequirement
        {
            AttributeName = "Bravery",
            MinValue = 60
        };

        TestOutputHelpers.LogCharacterState(_output, character, "Initial State");

        // Act
        var result = requirement.IsMet(character);

        // Assert
        TestOutputHelpers.LogPrerequisiteCheck(_output, requirement, character);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMet_WhenValueBelowMaximum_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Compassion = 75;

        var requirement = new AttributeRequirement
        {
            AttributeName = "Compassion",
            MaxValue = 80
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_WhenValueExactlyAtMaximum_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Compassion = 80;

        var requirement = new AttributeRequirement
        {
            AttributeName = "Compassion",
            MaxValue = 80
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_WhenValueAboveMaximum_ReturnsFalse()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Compassion = 85;

        var requirement = new AttributeRequirement
        {
            AttributeName = "Compassion",
            MaxValue = 80
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMet_WhenValueInRange_ReturnsTrue()
    {
        // Arrange
        TestOutputHelpers.LogScenario(_output, "Balanced Compassion Within Acceptable Range");

        var character = new Character();
        character.Attributes.Compassion = 65;

        var requirement = new AttributeRequirement
        {
            AttributeName = "Compassion",
            MinValue = 60,
            MaxValue = 80
        };

        TestOutputHelpers.LogCharacterState(_output, character, "Initial State");

        // Act
        var result = requirement.IsMet(character);

        // Assert
        TestOutputHelpers.LogPrerequisiteCheck(_output, requirement, character);
        _output.WriteLine("✓ Storylet Available: 'A Compassionate Approach'");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_WhenValueOutsideRange_ReturnsFalse()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Compassion = 90;

        var requirement = new AttributeRequirement
        {
            AttributeName = "Compassion",
            MinValue = 60,
            MaxValue = 80
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetDisplayText_WithMinValueOnly_ReturnsCorrectFormat()
    {
        // Arrange
        var requirement = new AttributeRequirement
        {
            AttributeName = "Bravery",
            MinValue = 60
        };

        // Act
        var result = requirement.GetDisplayText();

        // Assert
        result.Should().Be("Bravery ≥ 60");
    }

    [Fact]
    public void GetDisplayText_WithMaxValueOnly_ReturnsCorrectFormat()
    {
        // Arrange
        var requirement = new AttributeRequirement
        {
            AttributeName = "Compassion",
            MaxValue = 80
        };

        // Act
        var result = requirement.GetDisplayText();

        // Assert
        result.Should().Be("Compassion ≤ 80");
    }

    [Fact]
    public void GetDisplayText_WithBothMinAndMax_ReturnsCorrectFormat()
    {
        // Arrange
        var requirement = new AttributeRequirement
        {
            AttributeName = "Compassion",
            MinValue = 60,
            MaxValue = 80
        };

        // Act
        var result = requirement.GetDisplayText();

        // Assert
        result.Should().Be("Compassion between 60-80");
    }
}
