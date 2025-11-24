namespace Psyche.Tests.Data;

using System.IO;
using Psyche.Data;
using Psyche.Models;
using Psyche.Systems.Prerequisites;
using Psyche.Systems.Effects;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

public class JsonStoryletRepositoryTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly string _testDataDirectory;

    public JsonStoryletRepositoryTests(ITestOutputHelper output)
    {
        _output = output;
        // Create a unique temporary directory for each test
        _testDataDirectory = Path.Combine(Path.GetTempPath(), "psyche_tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDataDirectory);
        _output.WriteLine($"Test data directory: {_testDataDirectory}");
    }

    public void Dispose()
    {
        // Clean up test directory after tests
        if (Directory.Exists(_testDataDirectory))
        {
            Directory.Delete(_testDataDirectory, recursive: true);
        }
    }

    [Fact]
    public void Constructor_LoadsExistingStorylets()
    {
        // Arrange
        CreateTestStoryletFile("test1", "Test Storylet 1");
        CreateTestStoryletFile("test2", "Test Storylet 2");

        // Act
        var repository = new JsonStoryletRepository(_testDataDirectory);

        // Assert
        var all = repository.GetAll().ToList();
        all.Should().HaveCount(2);
        all.Should().Contain(s => s.Id == "test1");
        all.Should().Contain(s => s.Id == "test2");

        _output.WriteLine($"✓ Loaded {all.Count} storylets on construction");
    }

    [Fact]
    public void GetById_ExistingId_ReturnsStorylet()
    {
        // Arrange
        CreateTestStoryletFile("test_storylet", "Test");
        var repository = new JsonStoryletRepository(_testDataDirectory);

        // Act
        var storylet = repository.GetById("test_storylet");

        // Assert
        storylet.Should().NotBeNull();
        storylet!.Id.Should().Be("test_storylet");
        storylet.Title.Should().Be("Test");

        _output.WriteLine($"✓ Retrieved storylet by ID: {storylet.Id}");
    }

    [Fact]
    public void GetById_NonExistentId_ReturnsNull()
    {
        // Arrange
        var repository = new JsonStoryletRepository(_testDataDirectory);

        // Act
        var storylet = repository.GetById("nonexistent");

        // Assert
        storylet.Should().BeNull();

        _output.WriteLine("✓ Non-existent ID returns null");
    }

    [Fact]
    public void GetAll_ReturnsAllStorylets()
    {
        // Arrange
        CreateTestStoryletFile("story1", "Story 1");
        CreateTestStoryletFile("story2", "Story 2");
        CreateTestStoryletFile("story3", "Story 3");
        var repository = new JsonStoryletRepository(_testDataDirectory);

        // Act
        var all = repository.GetAll().ToList();

        // Assert
        all.Should().HaveCount(3);

        _output.WriteLine($"✓ GetAll returned {all.Count} storylets");
    }

    [Fact]
    public void GetByCategory_FiltersCorrectly()
    {
        // Arrange
        CreateTestStoryletFile("combat1", "Combat 1", category: "combat");
        CreateTestStoryletFile("combat2", "Combat 2", category: "combat");
        CreateTestStoryletFile("social1", "Social 1", category: "social");
        var repository = new JsonStoryletRepository(_testDataDirectory);

        // Act
        var combatStorylets = repository.GetByCategory("combat").ToList();

        // Assert
        combatStorylets.Should().HaveCount(2);
        combatStorylets.Should().AllSatisfy(s => s.Category.Should().Be("combat"));

        _output.WriteLine($"✓ Category filter 'combat' returned {combatStorylets.Count} storylets");
    }

    [Fact]
    public void GetByTags_ReturnsStoryletsWithAnyTag()
    {
        // Arrange
        CreateTestStoryletFile("story1", "Story 1", tags: new[] { "brave", "dangerous" });
        CreateTestStoryletFile("story2", "Story 2", tags: new[] { "social", "diplomatic" });
        CreateTestStoryletFile("story3", "Story 3", tags: new[] { "brave", "social" });
        var repository = new JsonStoryletRepository(_testDataDirectory);

        // Act
        var braveStorylets = repository.GetByTags("brave").ToList();
        var socialOrDiplomaticStorylets = repository.GetByTags("social", "diplomatic").ToList();

        // Assert
        braveStorylets.Should().HaveCount(2);
        socialOrDiplomaticStorylets.Should().HaveCount(2);

        _output.WriteLine($"✓ Tag filter 'brave' returned {braveStorylets.Count} storylets");
        _output.WriteLine($"✓ Tag filter 'social OR diplomatic' returned {socialOrDiplomaticStorylets.Count} storylets");
    }

    [Fact]
    public void Save_NewStorylet_AddsToRepository()
    {
        // Arrange
        var repository = new JsonStoryletRepository(_testDataDirectory);
        var newStorylet = new Storylet
        {
            Id = "new_storylet",
            Title = "New Storylet",
            Description = "A new storylet",
            Content = "Content",
            Category = "test"
        };

        // Act
        repository.Save(newStorylet);

        // Assert
        var retrieved = repository.GetById("new_storylet");
        retrieved.Should().NotBeNull();
        retrieved!.Title.Should().Be("New Storylet");

        // Verify file was created
        var filePath = Path.Combine(_testDataDirectory, "new_storylet.jsonc");
        File.Exists(filePath).Should().BeTrue();

        _output.WriteLine($"✓ Saved new storylet: {newStorylet.Id}");
    }

    [Fact]
    public void Save_InvalidStorylet_ThrowsException()
    {
        // Arrange
        var repository = new JsonStoryletRepository(_testDataDirectory);
        var invalidStorylet = new Storylet
        {
            Id = "", // Invalid: empty ID
            Title = "Invalid"
        };

        // Act
        var act = () => repository.Save(invalidStorylet);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot save invalid storylet*");

        _output.WriteLine("✓ Invalid storylet correctly throws exception");
    }

    [Fact]
    public void Save_UpdateExistingStorylet_UpdatesRepository()
    {
        // Arrange
        CreateTestStoryletFile("updateable", "Original Title");
        var repository = new JsonStoryletRepository(_testDataDirectory);
        var storylet = repository.GetById("updateable")!;
        storylet.Title = "Updated Title";

        // Act
        repository.Save(storylet);

        // Assert
        var retrieved = repository.GetById("updateable");
        retrieved!.Title.Should().Be("Updated Title");

        _output.WriteLine("✓ Updated existing storylet");
    }

    [Fact]
    public void Delete_ExistingStorylet_RemovesFromRepository()
    {
        // Arrange
        CreateTestStoryletFile("deletable", "Deletable");
        var repository = new JsonStoryletRepository(_testDataDirectory);

        // Act
        var result = repository.Delete("deletable");

        // Assert
        result.Should().BeTrue();
        repository.GetById("deletable").Should().BeNull();

        // Verify file was deleted
        var filePath = Path.Combine(_testDataDirectory, "deletable.jsonc");
        File.Exists(filePath).Should().BeFalse();

        _output.WriteLine("✓ Deleted storylet and file");
    }

    [Fact]
    public void Delete_NonExistentStorylet_ReturnsFalse()
    {
        // Arrange
        var repository = new JsonStoryletRepository(_testDataDirectory);

        // Act
        var result = repository.Delete("nonexistent");

        // Assert
        result.Should().BeFalse();

        _output.WriteLine("✓ Deleting non-existent storylet returns false");
    }

    [Fact]
    public void Reload_RefreshesFromDisk()
    {
        // Arrange
        CreateTestStoryletFile("story1", "Story 1");
        var repository = new JsonStoryletRepository(_testDataDirectory);
        repository.GetAll().Should().HaveCount(1);

        // Add a new file manually (simulating external change)
        CreateTestStoryletFile("story2", "Story 2");

        // Act
        repository.Reload();

        // Assert
        repository.GetAll().Should().HaveCount(2);
        repository.GetById("story2").Should().NotBeNull();

        _output.WriteLine("✓ Reload successfully refreshed from disk");
    }

    [Fact]
    public void Constructor_NonExistentDirectory_CreatesDirectory()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_testDataDirectory, "subdir", "storylets");
        Directory.Exists(nonExistentPath).Should().BeFalse();

        // Act
        var repository = new JsonStoryletRepository(nonExistentPath);

        // Assert
        Directory.Exists(nonExistentPath).Should().BeTrue();
        repository.GetAll().Should().BeEmpty();

        _output.WriteLine($"✓ Created non-existent directory: {nonExistentPath}");
    }

    [Fact]
    public void LoadStorylets_MalformedFile_SkipsAndContinues()
    {
        // Arrange
        CreateTestStoryletFile("good1", "Good 1");

        // Create a malformed file
        var badFilePath = Path.Combine(_testDataDirectory, "bad.jsonc");
        File.WriteAllText(badFilePath, "{ invalid json }");

        CreateTestStoryletFile("good2", "Good 2");

        // Act - Constructor should not throw
        var repository = new JsonStoryletRepository(_testDataDirectory);

        // Assert
        var all = repository.GetAll().ToList();
        all.Should().HaveCount(2); // Only good files loaded
        all.Should().Contain(s => s.Id == "good1");
        all.Should().Contain(s => s.Id == "good2");

        _output.WriteLine("✓ Malformed file skipped, other files loaded successfully");
    }

    // Helper method to create test storylet files
    private void CreateTestStoryletFile(string id, string title, string category = "test", string[]? tags = null)
    {
        var tagsJson = tags != null && tags.Length > 0
            ? string.Join(", ", tags.Select(t => $"\"{t}\""))
            : "";

        var jsonc = $$"""
        {
          "id": "{{id}}",
          "title": "{{title}}",
          "description": "Test description",
          "content": "Test content",
          "prerequisites": [],
          "effects": [],
          "options": [],
          "priority": 10,
          "category": "{{category}}",
          "tags": [{{tagsJson}}]
        }
        """;

        var filePath = Path.Combine(_testDataDirectory, $"{id}.jsonc");
        File.WriteAllText(filePath, jsonc);
    }
}
