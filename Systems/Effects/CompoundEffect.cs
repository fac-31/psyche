namespace Psyche.Systems.Effects;

using Psyche.Models.Mocks;

/// <summary>
/// Effect that applies multiple effects in sequence.
/// Example: Change Compassion +3, enemies_made -1, main_story_progress +1.
/// </summary>
public class CompoundEffect : IEffect
{
    /// <summary>The list of effects to apply in order.</summary>
    public List<IEffect> Effects { get; set; } = new();

    /// <summary>Applies all effects in sequence to the character.</summary>
    public void Apply(Character character)
    {
        foreach (var effect in Effects)
        {
            effect.Apply(character);
        }
    }

    /// <summary>Gets a human-readable description of all effects.</summary>
    public string GetDisplayText()
    {
        if (Effects.Count == 0)
            return "No effects";

        var parts = Effects.Select(e => e.GetDisplayText());
        return string.Join(", ", parts);
    }
}
