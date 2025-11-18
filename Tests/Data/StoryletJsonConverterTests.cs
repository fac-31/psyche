namespace Psyche.Tests.Data;

using System.Text.Json;
using Psyche.Data.Json;
using Psyche.Models;
using Psyche.Models.Mocks;
using Psyche.Systems.Prerequisites;
using Psyche.Systems.Effects;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

public class StoryletJsonConverterTests
{
    private readonly ITestOutputHelper _output;

    public StoryletJsonConverterTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Deserialize_SimpleStorylet_WithAttributeRequirement()
    {
        // Arrange
        var json = """
        {
          "id": "test_storylet",
          "title": "Test Storylet",
          "description": "A test",
          "content": "Test content",
          "prerequisites": [
            {
              "type": "AttributeRequirement",
              "properties": {
                "attributeName": "Bravery",
                "minValue": 60
              }
            }
          ],
          "effects": [],
          "options": [],
          "priority": 10,
          "category": "test",
          "tags": []
        }
        """;

        // Act
        var dto = JsonSerializer.Deserialize<StoryletDto>(json, StoryletJsonConverter.JsonOptions);
        var storylet = StoryletJsonConverter.FromDto(dto!);

        // Assert
        storylet.Id.Should().Be("test_storylet");
        storylet.Prerequisites.Should().HaveCount(1);
        storylet.Prerequisites[0].Should().BeOfType<AttributeRequirement>();

        var req = (AttributeRequirement)storylet.Prerequisites[0];
        req.AttributeName.Should().Be("Bravery");
        req.MinValue.Should().Be(60);

        _output.WriteLine($"✓ Deserialized storylet: {storylet.Title}");
        _output.WriteLine($"  Prerequisite: {req.GetDisplayText()}");
    }

    [Fact]
    public void Deserialize_JsoncWithComments_IgnoresComments()
    {
        // Arrange
        var jsonc = """
        { // This is a comment
          "id": "test", // Inline comment
          "title": "Test",
          "description": "Test", // Another comment
          "content": "Content",
          "prerequisites": [], // Empty array comment
          "effects": [],
          "options": [],
          "priority": 10,
          "category": "test",
          "tags": ["tag1", "tag2",] // Trailing comma
        }
        """;

        // Act
        var dto = JsonSerializer.Deserialize<StoryletDto>(jsonc, StoryletJsonConverter.JsonOptions);
        var storylet = StoryletJsonConverter.FromDto(dto!);

        // Assert
        storylet.Id.Should().Be("test");
        storylet.Tags.Should().Contain("tag1");
        storylet.Tags.Should().Contain("tag2");

        _output.WriteLine("✓ Successfully parsed JSONC with comments and trailing commas");
    }

    [Fact]
    public void Deserialize_StoryletWithOptions_CreatesAllOptions()
    {
        // Arrange
        var json = """
        {
          "id": "choice_storylet",
          "title": "A Choice",
          "description": "Choose wisely",
          "content": "What will you do?",
          "prerequisites": [],
          "effects": [],
          "options": [
            {
              "id": "option1",
              "text": "First choice",
              "description": "The first option",
              "resultText": "You chose first",
              "prerequisites": [],
              "effects": [
                {
                  "type": "AttributeEffect",
                  "properties": {
                    "attributeName": "Bravery",
                    "delta": 5
                  }
                }
              ],
              "priority": 10,
              "tags": []
            },
            {
              "id": "option2",
              "text": "Second choice",
              "description": "The second option",
              "resultText": "You chose second",
              "prerequisites": [],
              "effects": [],
              "priority": 5,
              "tags": []
            }
          ],
          "priority": 10,
          "category": "test",
          "tags": []
        }
        """;

        // Act
        var dto = JsonSerializer.Deserialize<StoryletDto>(json, StoryletJsonConverter.JsonOptions);
        var storylet = StoryletJsonConverter.FromDto(dto!);

        // Assert
        storylet.Options.Should().HaveCount(2);
        storylet.Options[0].Id.Should().Be("option1");
        storylet.Options[0].Effects.Should().HaveCount(1);
        storylet.Options[0].Effects[0].Should().BeOfType<AttributeEffect>();
        storylet.Options[1].Id.Should().Be("option2");

        _output.WriteLine($"✓ Deserialized storylet with {storylet.Options.Count} options");
    }

    [Fact]
    public void Deserialize_CompoundPrerequisite_WithNestedLogic()
    {
        // Arrange
        var json = """
        {
          "id": "complex",
          "title": "Complex",
          "description": "Test",
          "content": "Content",
          "prerequisites": [
            {
              "type": "CompoundPrerequisite",
              "properties": {
                "logic": "Or",
                "prerequisites": [
                  {
                    "type": "AttributeRequirement",
                    "properties": {
                      "attributeName": "Bravery",
                      "minValue": 60
                    }
                  },
                  {
                    "type": "AttributeRequirement",
                    "properties": {
                      "attributeName": "Discernment",
                      "minValue": 70
                    }
                  }
                ]
              }
            }
          ],
          "effects": [],
          "options": [],
          "priority": 10,
          "category": "test",
          "tags": []
        }
        """;

        // Act
        var dto = JsonSerializer.Deserialize<StoryletDto>(json, StoryletJsonConverter.JsonOptions);
        var storylet = StoryletJsonConverter.FromDto(dto!);

        // Assert
        storylet.Prerequisites.Should().HaveCount(1);
        storylet.Prerequisites[0].Should().BeOfType<CompoundPrerequisite>();

        var compound = (CompoundPrerequisite)storylet.Prerequisites[0];
        compound.Logic.Should().Be(CompoundPrerequisite.LogicType.Or);
        compound.Prerequisites.Should().HaveCount(2);
        compound.Prerequisites[0].Should().BeOfType<AttributeRequirement>();
        compound.Prerequisites[1].Should().BeOfType<AttributeRequirement>();

        _output.WriteLine($"✓ Compound prerequisite: {compound.GetDisplayText()}");
    }

    [Fact]
    public void Deserialize_AllEffectTypes_CreatesCorrectInstances()
    {
        // Arrange
        var json = """
        {
          "id": "effects_test",
          "title": "Effects Test",
          "description": "Test all effect types",
          "content": "Content",
          "prerequisites": [],
          "effects": [
            {
              "type": "AttributeEffect",
              "properties": {
                "attributeName": "Compassion",
                "delta": 3
              }
            },
            {
              "type": "QualityEffect",
              "properties": {
                "qualityId": "social_capital",
                "delta": 5
              }
            },
            {
              "type": "UnlockStoryletEffect",
              "properties": {
                "storyletId": "next_storylet"
              }
            },
            {
              "type": "CompoundEffect",
              "properties": {
                "effects": [
                  {
                    "type": "AttributeEffect",
                    "properties": {
                      "attributeName": "Bravery",
                      "delta": 2
                    }
                  }
                ]
              }
            }
          ],
          "options": [],
          "priority": 10,
          "category": "test",
          "tags": []
        }
        """;

        // Act
        var dto = JsonSerializer.Deserialize<StoryletDto>(json, StoryletJsonConverter.JsonOptions);
        var storylet = StoryletJsonConverter.FromDto(dto!);

        // Assert
        storylet.Effects.Should().HaveCount(4);
        storylet.Effects[0].Should().BeOfType<AttributeEffect>();
        storylet.Effects[1].Should().BeOfType<QualityEffect>();
        storylet.Effects[2].Should().BeOfType<UnlockStoryletEffect>();
        storylet.Effects[3].Should().BeOfType<CompoundEffect>();

        _output.WriteLine("✓ All effect types deserialized correctly:");
        foreach (var effect in storylet.Effects)
        {
            _output.WriteLine($"  - {effect.GetType().Name}: {effect.GetDisplayText()}");
        }
    }

    [Fact]
    public void Deserialize_QualityRequirement_WithMinAndMax()
    {
        // Arrange
        var json = """
        {
          "id": "quality_test",
          "title": "Quality Test",
          "description": "Test",
          "content": "Content",
          "prerequisites": [
            {
              "type": "QualityRequirement",
              "properties": {
                "qualityId": "psychological_strain",
                "minValue": 10,
                "maxValue": 50
              }
            }
          ],
          "effects": [],
          "options": [],
          "priority": 10,
          "category": "test",
          "tags": []
        }
        """;

        // Act
        var dto = JsonSerializer.Deserialize<StoryletDto>(json, StoryletJsonConverter.JsonOptions);
        var storylet = StoryletJsonConverter.FromDto(dto!);

        // Assert
        storylet.Prerequisites[0].Should().BeOfType<QualityRequirement>();
        var req = (QualityRequirement)storylet.Prerequisites[0];
        req.QualityId.Should().Be("psychological_strain");
        req.MinValue.Should().Be(10);
        req.MaxValue.Should().Be(50);

        _output.WriteLine($"✓ Quality requirement: {req.GetDisplayText()}");
    }

    [Fact]
    public void Deserialize_StoryletPlayedRequirement_Works()
    {
        // Arrange
        var json = """
        {
          "id": "sequel",
          "title": "Sequel",
          "description": "Test",
          "content": "Content",
          "prerequisites": [
            {
              "type": "StoryletPlayedRequirement",
              "properties": {
                "storyletId": "first_storylet",
                "mustHavePlayed": true
              }
            }
          ],
          "effects": [],
          "options": [],
          "priority": 10,
          "category": "test",
          "tags": []
        }
        """;

        // Act
        var dto = JsonSerializer.Deserialize<StoryletDto>(json, StoryletJsonConverter.JsonOptions);
        var storylet = StoryletJsonConverter.FromDto(dto!);

        // Assert
        storylet.Prerequisites[0].Should().BeOfType<StoryletPlayedRequirement>();
        var req = (StoryletPlayedRequirement)storylet.Prerequisites[0];
        req.StoryletId.Should().Be("first_storylet");
        req.MustHavePlayed.Should().BeTrue();

        _output.WriteLine($"✓ Storylet played requirement: {req.GetDisplayText()}");
    }

    [Fact]
    public void RoundTrip_SerializeAndDeserialize_MaintainsData()
    {
        // Arrange
        var originalStorylet = new Storylet
        {
            Id = "roundtrip_test",
            Title = "Round Trip Test",
            Description = "Testing serialization",
            Content = "Full content here",
            Priority = 15,
            Category = "test",
            Tags = new List<string> { "test", "roundtrip" },
            Prerequisites = new List<IPrerequisite>
            {
                new AttributeRequirement { AttributeName = "Bravery", MinValue = 50 }
            },
            Effects = new List<IEffect>
            {
                new AttributeEffect { AttributeName = "Bravery", Delta = 5 }
            },
            Options = new List<StoryletOption>
            {
                new StoryletOption
                {
                    Id = "choice1",
                    Text = "First choice",
                    Description = "Desc",
                    ResultText = "Result",
                    Priority = 10,
                    Tags = new List<string> { "brave" },
                    Prerequisites = new List<IPrerequisite>(),
                    Effects = new List<IEffect>
                    {
                        new QualityEffect { QualityId = "social_capital", Delta = 3 }
                    }
                }
            }
        };

        // Act
        var dto = StoryletJsonConverter.ToDto(originalStorylet);
        var json = JsonSerializer.Serialize(dto, StoryletJsonConverter.JsonOptions);
        _output.WriteLine("Serialized JSON:");
        _output.WriteLine(json);

        var deserializedDto = JsonSerializer.Deserialize<StoryletDto>(json, StoryletJsonConverter.JsonOptions);
        var roundTripStorylet = StoryletJsonConverter.FromDto(deserializedDto!);

        // Assert
        roundTripStorylet.Id.Should().Be(originalStorylet.Id);
        roundTripStorylet.Title.Should().Be(originalStorylet.Title);
        roundTripStorylet.Prerequisites.Should().HaveCount(1);
        roundTripStorylet.Effects.Should().HaveCount(1);
        roundTripStorylet.Options.Should().HaveCount(1);
        roundTripStorylet.Options[0].Effects.Should().HaveCount(1);
        roundTripStorylet.Tags.Should().BeEquivalentTo(originalStorylet.Tags);

        _output.WriteLine("\n✓ Round-trip successful - all data preserved");
    }

    [Fact]
    public void Deserialize_InvalidPrerequisiteType_ThrowsException()
    {
        // Arrange
        var json = """
        {
          "id": "invalid",
          "title": "Invalid",
          "description": "Test",
          "content": "Content",
          "prerequisites": [
            {
              "type": "UnknownPrerequisiteType",
              "properties": {}
            }
          ],
          "effects": [],
          "options": [],
          "priority": 10,
          "category": "test",
          "tags": []
        }
        """;

        // Act
        var dto = JsonSerializer.Deserialize<StoryletDto>(json, StoryletJsonConverter.JsonOptions);
        var act = () => StoryletJsonConverter.FromDto(dto!);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Unknown prerequisite type: UnknownPrerequisiteType");

        _output.WriteLine("✓ Invalid prerequisite type correctly throws exception");
    }

    [Fact]
    public void Deserialize_InvalidEffectType_ThrowsException()
    {
        // Arrange
        var json = """
        {
          "id": "invalid",
          "title": "Invalid",
          "description": "Test",
          "content": "Content",
          "prerequisites": [],
          "effects": [
            {
              "type": "UnknownEffectType",
              "properties": {}
            }
          ],
          "options": [],
          "priority": 10,
          "category": "test",
          "tags": []
        }
        """;

        // Act
        var dto = JsonSerializer.Deserialize<StoryletDto>(json, StoryletJsonConverter.JsonOptions);
        var act = () => StoryletJsonConverter.FromDto(dto!);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Unknown effect type: UnknownEffectType");

        _output.WriteLine("✓ Invalid effect type correctly throws exception");
    }

    [Fact]
    public void Deserialize_RealStoryletFile_LoadsSuccessfully()
    {
        // Arrange
        var filePath = "Data/Storylets/first_encounter.jsonc";

        // Skip test if file doesn't exist (for CI/CD environments)
        if (!File.Exists(filePath))
        {
            _output.WriteLine("⊘ Skipping test - storylet file not found");
            return;
        }

        // Act
        var jsonc = File.ReadAllText(filePath);
        var dto = JsonSerializer.Deserialize<StoryletDto>(jsonc, StoryletJsonConverter.JsonOptions);
        var storylet = StoryletJsonConverter.FromDto(dto!);

        // Assert
        storylet.Id.Should().Be("first_encounter");
        storylet.Title.Should().Be("A Chance Meeting");
        storylet.Options.Should().HaveCount(3);
        storylet.Options[0].Id.Should().Be("help");
        storylet.Options[1].Id.Should().Be("investigate");
        storylet.Options[2].Id.Should().Be("ignore");

        _output.WriteLine($"✓ Loaded real storylet file: {storylet.Title}");
        _output.WriteLine($"  Options: {string.Join(", ", storylet.Options.Select(o => o.Text))}");
    }
}
