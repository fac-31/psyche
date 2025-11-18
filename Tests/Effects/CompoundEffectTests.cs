namespace Psyche.Tests.Effects;

using Psyche.Systems.Effects;
using Psyche.Models.Mocks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

public class CompoundEffectTests
{
    private readonly ITestOutputHelper _output;

    public CompoundEffectTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Apply_AppliesAllEffectsInOrder()
    {
        // Arrange
        TestOutputHelpers.LogScenario(_output, "Storylet Choice: 'Show Mercy to Your Rival'");

        var character = new Character();
        character.Attributes.Compassion = 50;
        character.Qualities["enemies_made"] = 5;
        character.Qualities["main_story_progress"] = 0;

        TestOutputHelpers.LogCharacterState(_output, character, "Before Choice");

        var compound = new CompoundEffect
        {
            Effects = new List<IEffect>
            {
                new AttributeEffect { AttributeName = "Compassion", Delta = 3 },
                new QualityEffect { QualityId = "enemies_made", Delta = -1 },
                new QualityEffect { QualityId = "main_story_progress", Delta = 1 }
            }
        };

        _output.WriteLine("\n--- Storylet Executed ---");
        _output.WriteLine("Choice: Show mercy and spare your rival");
        _output.WriteLine($"Effects: {compound.GetDisplayText()}");

        // Create a snapshot for comparison
        var before = new Character
        {
            Attributes = new Psyche.Models.Mocks.CoreAttributes { Compassion = character.Attributes.Compassion },
            Qualities = new Dictionary<string, int>(character.Qualities)
        };

        // Act
        compound.Apply(character);

        // Assert
        TestOutputHelpers.LogEffectApplication(_output, compound, before, character);
        TestOutputHelpers.LogCharacterState(_output, character, "After Choice");

        character.Attributes.Compassion.Should().Be(53);
        character.GetQualityValue("enemies_made").Should().Be(4);
        character.GetQualityValue("main_story_progress").Should().Be(1);

        _output.WriteLine("\n✓ Story Progressed!");
        _output.WriteLine("  Your compassion has grown, and one less enemy walks the halls.");
    }

    [Fact]
    public void Apply_WithMultipleAttributeEffects_AppliesAll()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 50;
        character.Attributes.SelfAssurance = 50;

        var compound = new CompoundEffect
        {
            Effects = new List<IEffect>
            {
                new AttributeEffect { AttributeName = "Bravery", Delta = 5 },
                new AttributeEffect { AttributeName = "SelfAssurance", Delta = 2 }
            }
        };

        // Act
        compound.Apply(character);

        // Assert
        character.Attributes.Bravery.Should().Be(55);
        character.Attributes.SelfAssurance.Should().Be(52);
    }

    [Fact]
    public void Apply_WithUnlockEffect_MarksStoryletAsPlayed()
    {
        // Arrange
        TestOutputHelpers.LogScenario(_output, "Storylet Choice: 'Confront the Shadow' (Unlocks New Content)");

        var character = new Character();
        character.Attributes.Bravery = 50;

        TestOutputHelpers.LogCharacterState(_output, character, "Before Choice");

        var compound = new CompoundEffect
        {
            Effects = new List<IEffect>
            {
                new AttributeEffect { AttributeName = "Bravery", Delta = 5 },
                new UnlockStoryletEffect { StoryletId = "chapter_1_complete" }
            }
        };

        _output.WriteLine("\n--- Storylet Executed ---");
        _output.WriteLine("Choice: Face your fears and confront the shadow");
        _output.WriteLine($"Effects: {compound.GetDisplayText()}");

        var before = new Character
        {
            Attributes = new Psyche.Models.Mocks.CoreAttributes { Bravery = character.Attributes.Bravery },
            PlayedStoryletIds = new HashSet<string>(character.PlayedStoryletIds)
        };

        // Act
        compound.Apply(character);

        // Assert
        TestOutputHelpers.LogEffectApplication(_output, compound, before, character);
        TestOutputHelpers.LogCharacterState(_output, character, "After Choice");

        character.Attributes.Bravery.Should().Be(55);
        character.HasPlayedStorylet("chapter_1_complete").Should().BeTrue();

        _output.WriteLine("\n✓ New Storylets Unlocked!");
        _output.WriteLine("  - 'Chapter 2: The Aftermath' is now available");
        _output.WriteLine("  - 'Reflect on Your Journey' is now available");
    }

    [Fact]
    public void Apply_WithEmptyEffectList_DoesNothing()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 50;

        var compound = new CompoundEffect
        {
            Effects = new List<IEffect>()
        };

        // Act
        compound.Apply(character);

        // Assert
        character.Attributes.Bravery.Should().Be(50);
    }

    [Fact]
    public void Apply_WithNestedCompoundEffect_AppliesAllEffects()
    {
        // Arrange
        var character = new Character();
        character.Attributes.Bravery = 50;
        character.Attributes.Compassion = 50;

        var compound = new CompoundEffect
        {
            Effects = new List<IEffect>
            {
                new CompoundEffect
                {
                    Effects = new List<IEffect>
                    {
                        new AttributeEffect { AttributeName = "Bravery", Delta = 5 },
                        new AttributeEffect { AttributeName = "Compassion", Delta = 3 }
                    }
                },
                new QualityEffect { QualityId = "main_story_progress", Delta = 1 }
            }
        };

        // Act
        compound.Apply(character);

        // Assert
        character.Attributes.Bravery.Should().Be(55);
        character.Attributes.Compassion.Should().Be(53);
        character.GetQualityValue("main_story_progress").Should().Be(1);
    }

    [Fact]
    public void GetDisplayText_WithMultipleEffects_JoinsWithComma()
    {
        // Arrange
        var compound = new CompoundEffect
        {
            Effects = new List<IEffect>
            {
                new AttributeEffect { AttributeName = "Compassion", Delta = 3 },
                new QualityEffect { QualityId = "enemies_made", Delta = -1 },
                new QualityEffect { QualityId = "main_story_progress", Delta = 1 }
            }
        };

        // Act
        var result = compound.GetDisplayText();

        // Assert
        result.Should().Be("Compassion +3, enemies_made -1, main_story_progress +1");
    }

    [Fact]
    public void GetDisplayText_WithEmptyEffectList_ReturnsNoEffects()
    {
        // Arrange
        var compound = new CompoundEffect
        {
            Effects = new List<IEffect>()
        };

        // Act
        var result = compound.GetDisplayText();

        // Assert
        result.Should().Be("No effects");
    }
}
