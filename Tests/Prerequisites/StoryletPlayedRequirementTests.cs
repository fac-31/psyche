namespace Psyche.Tests.Prerequisites;

using Psyche.Systems.Prerequisites;
using Psyche.Models.Mocks;
using FluentAssertions;
using Xunit;

public class StoryletPlayedRequirementTests
{
    [Fact]
    public void IsMet_MustHavePlayed_WhenStoryletPlayed_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        character.MarkStoryletPlayed("intro_001");

        var requirement = new StoryletPlayedRequirement
        {
            StoryletId = "intro_001",
            MustHavePlayed = true
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_MustHavePlayed_WhenStoryletNotPlayed_ReturnsFalse()
    {
        // Arrange
        var character = new Character();
        // intro_001 not marked as played

        var requirement = new StoryletPlayedRequirement
        {
            StoryletId = "intro_001",
            MustHavePlayed = true
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMet_MustNotHavePlayed_WhenStoryletNotPlayed_ReturnsTrue()
    {
        // Arrange
        var character = new Character();
        // ending_good not marked as played

        var requirement = new StoryletPlayedRequirement
        {
            StoryletId = "ending_good",
            MustHavePlayed = false
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMet_MustNotHavePlayed_WhenStoryletPlayed_ReturnsFalse()
    {
        // Arrange
        var character = new Character();
        character.MarkStoryletPlayed("ending_good");

        var requirement = new StoryletPlayedRequirement
        {
            StoryletId = "ending_good",
            MustHavePlayed = false
        };

        // Act
        var result = requirement.IsMet(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetDisplayText_MustHavePlayed_ReturnsCorrectFormat()
    {
        // Arrange
        var requirement = new StoryletPlayedRequirement
        {
            StoryletId = "intro_001",
            MustHavePlayed = true
        };

        // Act
        var result = requirement.GetDisplayText();

        // Assert
        result.Should().Be("Requires: intro_001 played");
    }

    [Fact]
    public void GetDisplayText_MustNotHavePlayed_ReturnsCorrectFormat()
    {
        // Arrange
        var requirement = new StoryletPlayedRequirement
        {
            StoryletId = "ending_good",
            MustHavePlayed = false
        };

        // Act
        var result = requirement.GetDisplayText();

        // Assert
        result.Should().Be("Requires: ending_good not played");
    }
}
