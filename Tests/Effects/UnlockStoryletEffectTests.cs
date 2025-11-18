namespace Psyche.Tests.Effects;

using Psyche.Systems.Effects;
using Psyche.Models.Mocks;
using FluentAssertions;
using Xunit;

public class UnlockStoryletEffectTests
{
    [Fact]
    public void Apply_MarksStoryletAsPlayed()
    {
        // Arrange
        var character = new Character();
        var effect = new UnlockStoryletEffect
        {
            StoryletId = "chapter_1_complete"
        };

        // Act
        effect.Apply(character);

        // Assert
        character.HasPlayedStorylet("chapter_1_complete").Should().BeTrue();
    }

    [Fact]
    public void Apply_WhenStoryletAlreadyPlayed_RemainsPlayed()
    {
        // Arrange
        var character = new Character();
        character.MarkStoryletPlayed("chapter_1_complete");

        var effect = new UnlockStoryletEffect
        {
            StoryletId = "chapter_1_complete"
        };

        // Act
        effect.Apply(character);

        // Assert
        character.HasPlayedStorylet("chapter_1_complete").Should().BeTrue();
    }

    [Fact]
    public void Apply_DoesNotAffectOtherStorylets()
    {
        // Arrange
        var character = new Character();
        character.MarkStoryletPlayed("intro_001");

        var effect = new UnlockStoryletEffect
        {
            StoryletId = "chapter_1_complete"
        };

        // Act
        effect.Apply(character);

        // Assert
        character.HasPlayedStorylet("intro_001").Should().BeTrue();
        character.HasPlayedStorylet("chapter_1_complete").Should().BeTrue();
        character.HasPlayedStorylet("chapter_2_start").Should().BeFalse();
    }

    [Fact]
    public void GetDisplayText_ReturnsCorrectFormat()
    {
        // Arrange
        var effect = new UnlockStoryletEffect
        {
            StoryletId = "chapter_1_complete"
        };

        // Act
        var result = effect.GetDisplayText();

        // Assert
        result.Should().Be("Unlocks: chapter_1_complete");
    }
}
