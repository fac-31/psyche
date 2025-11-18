namespace Psyche.Tests.Models;

using Psyche.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

public class StoryletValidationTests
{
    private readonly ITestOutputHelper _output;

    public StoryletValidationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Validate_WithValidStoryletWithOptions_ReturnsValid()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "test_storylet",
            Title = "A Test Storylet",
            Options = new List<StoryletOption>
            {
                new StoryletOption { Id = "option1", Text = "First choice" },
                new StoryletOption { Id = "option2", Text = "Second choice" }
            }
        };

        // Act
        var result = storylet.Validate();

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _output.WriteLine($"Storylet '{storylet.Title}' validation: {(result.IsValid ? "✓ Valid" : "✗ Invalid")}");
    }

    [Fact]
    public void Validate_WithValidLegacyStorylet_ReturnsValid()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "legacy_storylet",
            Title = "Legacy Storylet",
            Description = "A storylet without options (legacy mode)",
            Effects = new List<Systems.Effects.IEffect>
            {
                new Systems.Effects.AttributeEffect { AttributeName = "Bravery", Delta = 5 }
            }
        };

        // Act
        var result = storylet.Validate();

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _output.WriteLine($"Legacy storylet '{storylet.Title}' validation: {(result.IsValid ? "✓ Valid" : "✗ Invalid")}");
    }

    [Fact]
    public void Validate_WithEmptyId_ReturnsInvalid()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "",
            Title = "Test Storylet"
        };

        // Act
        var result = storylet.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Storylet Id cannot be empty");
        _output.WriteLine($"Validation errors:");
        foreach (var error in result.Errors)
        {
            _output.WriteLine($"  - {error}");
        }
    }

    [Fact]
    public void Validate_WithEmptyTitle_ReturnsInvalid()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "test_storylet",
            Title = ""
        };

        // Act
        var result = storylet.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Storylet Title cannot be empty");
    }

    [Fact]
    public void Validate_WithOptionMissingId_ReturnsInvalid()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "test_storylet",
            Title = "Test Storylet",
            Options = new List<StoryletOption>
            {
                new StoryletOption { Id = "", Text = "First choice" }
            }
        };

        // Act
        var result = storylet.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Option 0 has empty Id");
        _output.WriteLine($"Validation errors:");
        foreach (var error in result.Errors)
        {
            _output.WriteLine($"  - {error}");
        }
    }

    [Fact]
    public void Validate_WithOptionMissingText_ReturnsInvalid()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "test_storylet",
            Title = "Test Storylet",
            Options = new List<StoryletOption>
            {
                new StoryletOption { Id = "option1", Text = "" }
            }
        };

        // Act
        var result = storylet.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Option 0 (option1) has empty Text");
    }

    [Fact]
    public void Validate_WithDuplicateOptionIds_ReturnsInvalid()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "test_storylet",
            Title = "Test Storylet",
            Options = new List<StoryletOption>
            {
                new StoryletOption { Id = "same_id", Text = "First choice" },
                new StoryletOption { Id = "same_id", Text = "Second choice" },
                new StoryletOption { Id = "unique_id", Text = "Third choice" }
            }
        };

        // Act
        var result = storylet.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Duplicate option Id: same_id");
        _output.WriteLine($"Validation errors:");
        foreach (var error in result.Errors)
        {
            _output.WriteLine($"  - {error}");
        }
    }

    [Fact]
    public void Validate_WithMultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "",
            Title = "",
            Options = new List<StoryletOption>
            {
                new StoryletOption { Id = "", Text = "" },
                new StoryletOption { Id = "dup", Text = "Choice 1" },
                new StoryletOption { Id = "dup", Text = "Choice 2" }
            }
        };

        // Act
        var result = storylet.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterOrEqualTo(5);
        result.Errors.Should().Contain("Storylet Id cannot be empty");
        result.Errors.Should().Contain("Storylet Title cannot be empty");
        result.Errors.Should().Contain("Option 0 has empty Id");
        result.Errors.Should().Contain("Option 0 () has empty Text");
        result.Errors.Should().Contain("Duplicate option Id: dup");
        _output.WriteLine($"Found {result.Errors.Count} validation errors:");
        foreach (var error in result.Errors)
        {
            _output.WriteLine($"  - {error}");
        }
    }

    [Fact]
    public void Validate_EmptyStoryletNoOptionsNoEffects_ReturnsValid()
    {
        // Arrange
        var storylet = new Storylet
        {
            Id = "empty_storylet",
            Title = "Empty Storylet"
        };

        // Act
        var result = storylet.Validate();

        // Assert
        result.IsValid.Should().BeTrue("validation allows storylets without options or effects");
        _output.WriteLine("Note: Empty storylet is valid but may warrant a warning in production");
    }
}
