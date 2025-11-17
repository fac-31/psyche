namespace Psyche.Systems.Prerequisites;

using Psyche.Models.Mocks;

/// <summary>
/// Prerequisite that checks if a core attribute meets specified bounds.
/// Example: Requires Bravery >= 60, or Compassion between 60-80.
/// </summary>
public class AttributeRequirement : IPrerequisite
{
    /// <summary>The name of the attribute to check (e.g., "Bravery", "Compassion").</summary>
    public string AttributeName { get; set; } = string.Empty;

    /// <summary>Minimum required value (inclusive). Null means no minimum.</summary>
    public int? MinValue { get; set; }

    /// <summary>Maximum allowed value (inclusive). Null means no maximum.</summary>
    public int? MaxValue { get; set; }

    /// <summary>Evaluates whether the character's attribute meets the specified bounds.</summary>
    public bool IsMet(Character character)
    {
        var value = character.Attributes.GetAttributeValue(AttributeName);

        if (MinValue.HasValue && value < MinValue.Value)
            return false;

        if (MaxValue.HasValue && value > MaxValue.Value)
            return false;

        return true;
    }

    /// <summary>Gets a human-readable description of this requirement.</summary>
    public string GetDisplayText()
    {
        if (MinValue.HasValue && MaxValue.HasValue)
            return $"{AttributeName} between {MinValue}-{MaxValue}";
        else if (MinValue.HasValue)
            return $"{AttributeName} ≥ {MinValue}";
        else if (MaxValue.HasValue)
            return $"{AttributeName} ≤ {MaxValue}";
        else
            return $"{AttributeName} (any value)";
    }
}
