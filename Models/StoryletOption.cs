namespace Psyche.Models;

using Psyche.Systems.Prerequisites;
using Psyche.Systems.Effects;

/// <summary>
/// Represents a choice within a storylet.
/// Each option has its own prerequisites (visibility/availability) and effects (consequences).
/// </summary>
public class StoryletOption
{
    /// <summary>Unique identifier for this option within the storylet.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Text displayed for this choice option.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description shown when option is selected/hovered.
    /// Can describe consequences or provide narrative context.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Narrative content displayed when this option is chosen.
    /// This is the outcome text the player sees.
    /// </summary>
    public string ResultText { get; set; } = string.Empty;

    /// <summary>
    /// Conditions that must be met for this option to be available.
    /// If not met, option may be hidden or shown as locked.
    /// </summary>
    public List<IPrerequisite> Prerequisites { get; set; } = new();

    /// <summary>
    /// Changes applied to character state when this option is chosen.
    /// </summary>
    public List<IEffect> Effects { get; set; } = new();

    /// <summary>
    /// Display priority within the option list (higher = shown first).
    /// Default: 10. Useful for controlling choice order.
    /// </summary>
    public int Priority { get; set; } = 10;

    /// <summary>
    /// Tags for categorizing options (e.g., "aggressive", "diplomatic", "risky").
    /// Can be used for analytics or special UI treatment.
    /// </summary>
    public List<string> Tags { get; set; } = new();
}
