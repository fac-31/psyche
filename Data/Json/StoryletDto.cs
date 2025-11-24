namespace Psyche.Data.Json;

using System.Text.Json.Serialization;

/// <summary>
/// Data transfer object for JSON serialization of storylets.
/// Maps to the Storylet model but uses primitive types for serialization.
/// </summary>
public class StoryletDto
{
    /// <summary>Unique identifier for this storylet.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Short title displayed in storylet lists.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Brief description shown before selection.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Full narrative content displayed when storylet is executed.</summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>Conditions that must be met for this storylet to be available.</summary>
    [JsonPropertyName("prerequisites")]
    public List<PrerequisiteDto> Prerequisites { get; set; } = new();

    /// <summary>Changes applied to character state when storylet is executed.</summary>
    [JsonPropertyName("effects")]
    public List<EffectDto> Effects { get; set; } = new();

    /// <summary>Available choices within this storylet.</summary>
    [JsonPropertyName("options")]
    public List<StoryletOptionDto> Options { get; set; } = new();

    /// <summary>Display priority (higher = shown first). Default: 10.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; } = 10;

    /// <summary>Storylet category (e.g., "exploration", "combat", "social").</summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>Tags for filtering and searching.</summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Data transfer object for storylet option serialization.
/// Represents a choice within a storylet.
/// </summary>
public class StoryletOptionDto
{
    /// <summary>Unique identifier for this option within the storylet.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Text displayed for this choice option.</summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>Detailed description shown when option is selected/hovered.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Narrative content displayed when this option is chosen.</summary>
    [JsonPropertyName("resultText")]
    public string ResultText { get; set; } = string.Empty;

    /// <summary>Conditions that must be met for this option to be available.</summary>
    [JsonPropertyName("prerequisites")]
    public List<PrerequisiteDto> Prerequisites { get; set; } = new();

    /// <summary>Changes applied to character state when this option is chosen.</summary>
    [JsonPropertyName("effects")]
    public List<EffectDto> Effects { get; set; } = new();

    /// <summary>Display priority within the option list (higher = shown first).</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; } = 10;

    /// <summary>Tags for categorizing options.</summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Data transfer object for prerequisite serialization.
/// Uses type discriminator pattern for polymorphic deserialization.
/// </summary>
public class PrerequisiteDto
{
    /// <summary>Type discriminator (e.g., "AttributeRequirement", "QualityRequirement").</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Prerequisite-specific properties stored as key-value pairs.</summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Data transfer object for effect serialization.
/// Uses type discriminator pattern for polymorphic deserialization.
/// </summary>
public class EffectDto
{
    /// <summary>Type discriminator (e.g., "AttributeEffect", "QualityEffect").</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Effect-specific properties stored as key-value pairs.</summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}
