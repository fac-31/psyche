namespace Psyche.Tests.Models;

using Psyche.Models;
using Psyche.Models.Mocks;
using Psyche.Systems.Prerequisites;
using Psyche.Systems.Effects;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

public class StoryletOptionTests
{
    private readonly ITestOutputHelper _output;

    public StoryletOptionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Constructor_CreatesOptionWithDefaults()
    {
        // Arrange & Act
        var option = new StoryletOption();

        // Assert
        option.Id.Should().Be(string.Empty);
        option.Text.Should().Be(string.Empty);
        option.Description.Should().Be(string.Empty);
        option.ResultText.Should().Be(string.Empty);
        option.Prerequisites.Should().BeEmpty();
        option.Effects.Should().BeEmpty();
        option.Priority.Should().Be(10);
        option.Tags.Should().BeEmpty();
    }

    [Fact]
    public void Option_WithProperties_StoresValuesCorrectly()
    {
        // Arrange & Act
        var option = new StoryletOption
        {
            Id = "mercy",
            Text = "Show mercy",
            Description = "Spare your rival",
            ResultText = "You lower your weapon...",
            Priority = 15,
            Tags = new List<string> { "compassionate", "diplomatic" }
        };

        // Assert
        option.Id.Should().Be("mercy");
        option.Text.Should().Be("Show mercy");
        option.Description.Should().Be("Spare your rival");
        option.ResultText.Should().Be("You lower your weapon...");
        option.Priority.Should().Be(15);
        option.Tags.Should().HaveCount(2);
        option.Tags.Should().Contain("compassionate");
        option.Tags.Should().Contain("diplomatic");
    }

    [Fact]
    public void Option_WithPrerequisites_EvaluatesCorrectly()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Compassion = 70;

        var option = new StoryletOption
        {
            Id = "mercy",
            Text = "Show mercy",
            Prerequisites = new List<IPrerequisite>
            {
                new AttributeRequirement { AttributeName = "Compassion", MinValue = 60 }
            }
        };

        // Act
        var isMet = option.Prerequisites.All(prereq => prereq.IsMet(character));

        // Assert
        isMet.Should().BeTrue();
        _output.WriteLine($"Character Compassion: {character.Attributes.Compassion}");
        _output.WriteLine($"Option '{option.Text}' is available: {isMet}");
    }

    [Fact]
    public void Option_WithUnmetPrerequisites_IsNotAvailable()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 30; // Too low

        var option = new StoryletOption
        {
            Id = "heroic_charge",
            Text = "Lead a heroic charge",
            Prerequisites = new List<IPrerequisite>
            {
                new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 }
            }
        };

        // Act
        var isMet = option.Prerequisites.All(prereq => prereq.IsMet(character));

        // Assert
        isMet.Should().BeFalse();
        _output.WriteLine($"Character Bravery: {character.Attributes.Bravery}");
        _output.WriteLine($"Option '{option.Text}' is available: {isMet}");
    }

    [Fact]
    public void Option_WithEffects_AppliesCorrectly()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Compassion = 50;
        character.Qualities["social_capital"] = 10;

        var option = new StoryletOption
        {
            Id = "mercy",
            Text = "Show mercy",
            Effects = new List<IEffect>
            {
                new AttributeEffect { AttributeName = "Compassion", Delta = 5 },
                new QualityEffect { QualityId = "social_capital", Delta = 3 }
            }
        };

        // Act
        foreach (var effect in option.Effects)
        {
            effect.Apply(character);
        }

        // Assert
        character.Attributes.Compassion.Should().Be(55);
        character.Qualities["social_capital"].Should().Be(13);
        _output.WriteLine($"After choosing '{option.Text}':");
        _output.WriteLine($"  Compassion: 50 → {character.Attributes.Compassion}");
        _output.WriteLine($"  Social Capital: 10 → {character.Qualities["social_capital"]}");
    }

    [Fact]
    public void Option_WithMultiplePrerequisites_RequiresAllMet()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 70;
        character.Attributes.Compassion = 40; // Too low
        character.Qualities["social_capital"] = 5;

        var option = new StoryletOption
        {
            Id = "balanced_approach",
            Text = "Take a balanced approach",
            Prerequisites = new List<IPrerequisite>
            {
                new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
                new AttributeRequirement { AttributeName = "Compassion", MinValue = 50 },
                new QualityRequirement { QualityId = "social_capital", MinValue = 3 }
            }
        };

        // Act
        var isMet = option.Prerequisites.All(prereq => prereq.IsMet(character));

        // Assert
        isMet.Should().BeFalse("Compassion requirement is not met");
        _output.WriteLine($"Character state:");
        _output.WriteLine($"  Bravery: {character.Attributes.Bravery} (requires 60) ✓");
        _output.WriteLine($"  Compassion: {character.Attributes.Compassion} (requires 50) ✗");
        _output.WriteLine($"  Social Capital: {character.Qualities["social_capital"]} (requires 3) ✓");
    }

    [Fact]
    public void Option_WithCompoundEffect_AppliesAllChanges()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 50;
        character.Qualities["psychological_strain"] = 30;
        character.Qualities["main_story_progress"] = 1;

        var option = new StoryletOption
        {
            Id = "dangerous_path",
            Text = "Take the dangerous path",
            Effects = new List<IEffect>
            {
                new CompoundEffect
                {
                    Effects = new List<IEffect>
                    {
                        new AttributeEffect { AttributeName = "Bravery", Delta = 5 },
                        new QualityEffect { QualityId = "psychological_strain", Delta = 10 },
                        new QualityEffect { QualityId = "main_story_progress", Delta = 1 }
                    }
                }
            }
        };

        // Act
        foreach (var effect in option.Effects)
        {
            effect.Apply(character);
        }

        // Assert
        character.Attributes.Bravery.Should().Be(55);
        character.Qualities["psychological_strain"].Should().Be(40);
        character.Qualities["main_story_progress"].Should().Be(2);
        _output.WriteLine($"After choosing '{option.Text}':");
        _output.WriteLine($"  Bravery: 50 → {character.Attributes.Bravery}");
        _output.WriteLine($"  Psychological Strain: 30 → {character.Qualities["psychological_strain"]}");
        _output.WriteLine($"  Story Progress: 1 → {character.Qualities["main_story_progress"]}");
    }
}
