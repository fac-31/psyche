namespace Psyche.Tests.Prerequisites;

using Psyche.Systems.Prerequisites;
using Psyche.Models.Mocks;
using FluentAssertions;
using Xunit;

public class CompoundPrerequisiteTests
{
    [Fact]
    public void IsMet_AndLogic_WhenAllPrerequisitesMet_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 65;
        character.Attributes.Compassion = 70;

        var compound = new CompoundPrerequisite
        {
            Logic = CompoundPrerequisite.LogicType.And,
            Prerequisites = new List<IPrerequisite>
            {
                new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
                new AttributeRequirement { AttributeName = "Compassion", MinValue = 60 }
            }
        };

        // Act
        var result = compound.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_AndLogic_WhenOnePrerequisiteNotMet_ReturnsFalse()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 65;
        character.Attributes.Compassion = 45; // Below minimum

        var compound = new CompoundPrerequisite
        {
            Logic = CompoundPrerequisite.LogicType.And,
            Prerequisites = new List<IPrerequisite>
            {
                new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
                new AttributeRequirement { AttributeName = "Compassion", MinValue = 60 }
            }
        };

        // Act
        var result = compound.IsMet(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMet_OrLogic_WhenAtLeastOnePrerequisiteMet_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 65;
        character.Attributes.Discernment = 55; // Below minimum

        var compound = new CompoundPrerequisite
        {
            Logic = CompoundPrerequisite.LogicType.Or,
            Prerequisites = new List<IPrerequisite>
            {
                new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
                new AttributeRequirement { AttributeName = "Discernment", MinValue = 70 }
            }
        };

        // Act
        var result = compound.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_OrLogic_WhenNoPrerequisitesMet_ReturnsFalse()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 55; // Below minimum
        character.Attributes.Discernment = 65; // Below minimum

        var compound = new CompoundPrerequisite
        {
            Logic = CompoundPrerequisite.LogicType.Or,
            Prerequisites = new List<IPrerequisite>
            {
                new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
                new AttributeRequirement { AttributeName = "Discernment", MinValue = 70 }
            }
        };

        // Act
        var result = compound.IsMet(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMet_NestedCompound_ReturnsCorrectResult()
    {
        // (Bravery >= 60 OR Discernment >= 70) AND social_capital >= 5
        var character = new Character();
        character.Attributes.Bravery = 65;
        character.Attributes.Discernment = 50;
        character.Qualities["social_capital"] = 10;

        var compound = new CompoundPrerequisite
        {
            Logic = CompoundPrerequisite.LogicType.And,
            Prerequisites = new List<IPrerequisite>
            {
                new CompoundPrerequisite
                {
                    Logic = CompoundPrerequisite.LogicType.Or,
                    Prerequisites = new List<IPrerequisite>
                    {
                        new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
                        new AttributeRequirement { AttributeName = "Discernment", MinValue = 70 }
                    }
                },
                new QualityRequirement { QualityId = "social_capital", MinValue = 5 }
            }
        };

        // Act
        var result = compound.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_EmptyPrerequisiteList_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        var compound = new CompoundPrerequisite
        {
            Logic = CompoundPrerequisite.LogicType.And,
            Prerequisites = new List<IPrerequisite>()
        };

        // Act
        var result = compound.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetDisplayText_AndLogic_ReturnsCorrectFormat()
    {
        // Arrange
        var compound = new CompoundPrerequisite
        {
            Logic = CompoundPrerequisite.LogicType.And,
            Prerequisites = new List<IPrerequisite>
            {
                new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
                new QualityRequirement { QualityId = "social_capital", MinValue = 5 }
            }
        };

        // Act
        var result = compound.GetDisplayText();

        // Assert
        result.Should().Be("(Bravery ≥ 60 AND social_capital ≥ 5)");
    }

    [Fact]
    public void GetDisplayText_OrLogic_ReturnsCorrectFormat()
    {
        // Arrange
        var compound = new CompoundPrerequisite
        {
            Logic = CompoundPrerequisite.LogicType.Or,
            Prerequisites = new List<IPrerequisite>
            {
                new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
                new AttributeRequirement { AttributeName = "Discernment", MinValue = 70 }
            }
        };

        // Act
        var result = compound.GetDisplayText();

        // Assert
        result.Should().Be("(Bravery ≥ 60 OR Discernment ≥ 70)");
    }
}
