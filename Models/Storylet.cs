namespace Psyche.Models;

using Psyche.Systems.Prerequisites;
using Psyche.Systems.Effects;

/// <summary>
/// Represents a narrative content unit with prerequisites and effects.
/// Based on quality-based narrative design by Emily Short.
/// </summary>
public class Storylet
{
    /// <summary>Unique identifier for this storylet.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Short title displayed in storylet lists.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Brief description shown before selection.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Full narrative content displayed when storylet is executed.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Conditions that must be met for this storylet to be available.</summary>
    public List<IPrerequisite> Prerequisites { get; set; } = new();

    /// <summary>Changes applied to character state when storylet is executed.</summary>
    public List<IEffect> Effects { get; set; } = new();

    /// <summary>Display priority (higher = shown first). Default: 10.</summary>
    public int Priority { get; set; } = 10;

    /// <summary>Category for organizing storylets (e.g., "exploration", "combat", "social").</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Tags for filtering and searching.</summary>
    public List<string> Tags { get; set; } = new();
}
