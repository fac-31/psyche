namespace Psyche.Systems.Effects;

using Psyche.Models.Mocks;

/// <summary>
/// Effect that modifies a core attribute value.
/// Automatically clamps to valid range (0-100).
/// Example: Increase Bravery by 5, or decrease Self-Assurance by 3.
/// </summary>
public class AttributeEffect : IEffect
{
    /// <summary>The name of the attribute to modify (e.g., "Bravery", "Compassion").</summary>
    public string AttributeName { get; set; } = string.Empty;

    /// <summary>The amount to change the attribute (positive for increase, negative for decrease).</summary>
    public int Delta { get; set; }

    /// <summary>Applies the attribute modification to the character.</summary>
    public void Apply(Character character)
    {
        character.Attributes.ModifyAttribute(AttributeName, Delta);
    }

    /// <summary>Gets a human-readable description of this effect.</summary>
    public string GetDisplayText()
    {
        var sign = Delta >= 0 ? "+" : "";
        return $"{AttributeName} {sign}{Delta}";
    }
}
