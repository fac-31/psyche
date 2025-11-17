namespace Psyche.Systems.Effects;

using Psyche.Models.Mocks;

/// <summary>
/// Effect that modifies a storylet quality value.
/// Example: Decrease psychological_strain by 10, or increase main_story_progress by 1.
/// </summary>
public class QualityEffect : IEffect
{
    /// <summary>The ID of the quality to modify (e.g., "social_capital", "psychological_strain").</summary>
    public string QualityId { get; set; } = string.Empty;

    /// <summary>The amount to change the quality (positive for increase, negative for decrease).</summary>
    public int Delta { get; set; }

    /// <summary>Applies the quality modification to the character.</summary>
    public void Apply(Character character)
    {
        character.ModifyQuality(QualityId, Delta);
    }

    /// <summary>Gets a human-readable description of this effect.</summary>
    public string GetDisplayText()
    {
        var sign = Delta >= 0 ? "+" : "";
        return $"{QualityId} {sign}{Delta}";
    }
}
