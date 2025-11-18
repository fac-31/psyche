namespace Psyche.Tests.Models;

using Psyche.Models;
using Psyche.Models.Mocks;
using Psyche.Systems.Prerequisites;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

public class StoryletOptionsAvailabilityTests
{
    private readonly ITestOutputHelper _output;

    public StoryletOptionsAvailabilityTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void GetAvailableOptions_WithNoPrerequisites_ReturnsAllOptions()
    {
        // Arrange
        var character = new Character();
        var storylet = new Storylet
        {
            Id = "test",
            Title = "Test",
            Options = new List<StoryletOption>
            {
                new StoryletOption { Id = "opt1", Text = "Option 1" },
                new StoryletOption { Id = "opt2", Text = "Option 2" },
                new StoryletOption { Id = "opt3", Text = "Option 3" }
            }
        };

        // Act
        var available = storylet.GetAvailableOptions(character);

        // Assert
        available.Should().HaveCount(3);
        _output.WriteLine($"All {available.Count} options are available (no prerequisites)");
    }

    [Fact]
    public void GetAvailableOptions_WithMetPrerequisites_ReturnsMatchingOptions()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Compassion = 70;
        character.Attributes.Bravery = 40;

        var storylet = new Storylet
        {
            Id = "moral_choice",
            Title = "A Moral Choice",
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "compassionate",
                    Text = "Show compassion",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new AttributeRequirement { AttributeName = "Compassion", MinValue = 60 }
                    }
                },
                new StoryletOption
                {
                    Id = "brave",
                    Text = "Be brave",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 }
                    }
                },
                new StoryletOption
                {
                    Id = "neutral",
                    Text = "Stay neutral"
                    // No prerequisites
                }
            }
        };

        // Act
        var available = storylet.GetAvailableOptions(character);

        // Assert
        available.Should().HaveCount(2);
        available.Should().Contain(opt => opt.Id == "compassionate");
        available.Should().Contain(opt => opt.Id == "neutral");
        available.Should().NotContain(opt => opt.Id == "brave");

        _output.WriteLine($"Character state:");
        _output.WriteLine($"  Compassion: {character.Attributes.Compassion} (requires 60) ✓");
        _output.WriteLine($"  Bravery: {character.Attributes.Bravery} (requires 60) ✗");
        _output.WriteLine($"Available options: {string.Join(", ", available.Select(o => o.Text))}");
    }

    [Fact]
    public void GetAvailableOptions_OrdersByPriority()
    {
        // Arrange
        var character = new Character();
        var storylet = new Storylet
        {
            Id = "test",
            Title = "Test",
            Options = new List<StoryletOption>
            {
                new StoryletOption { Id = "low", Text = "Low Priority", Priority = 5 },
                new StoryletOption { Id = "high", Text = "High Priority", Priority = 20 },
                new StoryletOption { Id = "medium", Text = "Medium Priority", Priority = 10 }
            }
        };

        // Act
        var available = storylet.GetAvailableOptions(character);

        // Assert
        available.Should().HaveCount(3);
        available[0].Id.Should().Be("high");
        available[1].Id.Should().Be("medium");
        available[2].Id.Should().Be("low");

        _output.WriteLine("Options ordered by priority:");
        for (int i = 0; i < available.Count; i++)
        {
            _output.WriteLine($"  {i + 1}. {available[i].Text} (Priority: {available[i].Priority})");
        }
    }

    [Fact]
    public void GetAvailableOptions_WithQualityRequirements_FiltersCorrectly()
    {
        // Arrange
        var character = new Character();
        character.Qualities["social_capital"] = 15;
        character.Qualities["secrets_learned"] = 5;

        var storylet = new Storylet
        {
            Id = "social_event",
            Title = "A Social Event",
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "leverage_connections",
                    Text = "Leverage your connections",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new QualityRequirement { QualityId = "social_capital", MinValue = 10 }
                    }
                },
                new StoryletOption
                {
                    Id = "use_secrets",
                    Text = "Use your secrets",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new QualityRequirement { QualityId = "secrets_learned", MinValue = 10 }
                    }
                },
                new StoryletOption
                {
                    Id = "observe",
                    Text = "Just observe"
                }
            }
        };

        // Act
        var available = storylet.GetAvailableOptions(character);

        // Assert
        available.Should().HaveCount(2);
        available.Should().Contain(opt => opt.Id == "leverage_connections");
        available.Should().Contain(opt => opt.Id == "observe");
        available.Should().NotContain(opt => opt.Id == "use_secrets");

        _output.WriteLine($"Character qualities:");
        _output.WriteLine($"  Social Capital: {character.Qualities["social_capital"]} (requires 10) ✓");
        _output.WriteLine($"  Secrets Learned: {character.Qualities["secrets_learned"]} (requires 10) ✗");
        _output.WriteLine($"Available options: {string.Join(", ", available.Select(o => o.Text))}");
    }

    [Fact]
    public void GetAvailableOptions_WithCompoundPrerequisites_EvaluatesCorrectly()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 70;
        character.Attributes.Discernment = 50;
        character.Qualities["social_capital"] = 8;

        var storylet = new Storylet
        {
            Id = "complex_choice",
            Title = "A Complex Decision",
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "brave_or_discerning",
                    Text = "Use your wits or courage",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new CompoundPrerequisite
                        {
                            Logic = CompoundLogic.Or,
                            Prerequisites = new List<IPrerequisite>
                            {
                                new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
                                new AttributeRequirement { AttributeName = "Discernment", MinValue = 70 }
                            }
                        }
                    }
                },
                new StoryletOption
                {
                    Id = "diplomatic_with_resources",
                    Text = "Diplomatic approach",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new CompoundPrerequisite
                        {
                            Logic = CompoundLogic.And,
                            Prerequisites = new List<IPrerequisite>
                            {
                                new AttributeRequirement { AttributeName = "Compassion", MinValue = 60 },
                                new QualityRequirement { QualityId = "social_capital", MinValue = 10 }
                            }
                        }
                    }
                }
            }
        };

        // Act
        var available = storylet.GetAvailableOptions(character);

        // Assert
        available.Should().HaveCount(1);
        available[0].Id.Should().Be("brave_or_discerning",
            "Bravery 70 meets requirement (OR logic), but diplomatic option fails both Compassion and social capital");

        _output.WriteLine($"Character state:");
        _output.WriteLine($"  Bravery: {character.Attributes.Bravery} (OR requires 60) ✓");
        _output.WriteLine($"  Discernment: {character.Attributes.Discernment} (OR requires 70) ✗");
        _output.WriteLine($"  Compassion: {character.Attributes.Compassion} (AND requires 60) ✗");
        _output.WriteLine($"  Social Capital: {character.Qualities["social_capital"]} (AND requires 10) ✗");
        _output.WriteLine($"Available options: {string.Join(", ", available.Select(o => o.Text))}");
    }

    [Fact]
    public void GetAvailableOptions_WhenNoOptionsAvailable_ReturnsEmptyList()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 20;
        character.Attributes.Compassion = 20;

        var storylet = new Storylet
        {
            Id = "locked_storylet",
            Title = "All Locked",
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "opt1",
                    Text = "Brave option",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 }
                    }
                },
                new StoryletOption
                {
                    Id = "opt2",
                    Text = "Compassionate option",
                    Prerequisites = new List<IPrerequisite>
                    {
                        new AttributeRequirement { AttributeName = "Compassion", MinValue = 60 }
                    }
                }
            }
        };

        // Act
        var available = storylet.GetAvailableOptions(character);

        // Assert
        available.Should().BeEmpty();
        _output.WriteLine("No options available - all prerequisites failed");
    }

    [Fact]
    public void GetAvailableOptions_WithEmptyOptions_ReturnsEmptyList()
    {
        // Arrange
        var character = new Character();
        var storylet = new Storylet
        {
            Id = "legacy",
            Title = "Legacy Storylet"
            // No options (legacy mode)
        };

        // Act
        var available = storylet.GetAvailableOptions(character);

        // Assert
        available.Should().BeEmpty();
        _output.WriteLine("Legacy storylet with no options returns empty list");
    }

    [Fact]
    public void HasChoices_WithOptions_ReturnsTrue()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "test",
            Title = "Test",
            Options = new List<StoryletOption>
            {
                new StoryletOption { Id = "opt1", Text = "Option 1" }
            }
        };

        // Act & Assert
        storylet.HasChoices.Should().BeTrue();
    }

    [Fact]
    public void HasChoices_WithoutOptions_ReturnsFalse()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "legacy",
            Title = "Legacy"
        };

        // Act & Assert
        storylet.HasChoices.Should().BeFalse();
    }
}
