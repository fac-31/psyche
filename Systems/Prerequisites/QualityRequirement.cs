namespace Psyche.Systems.Prerequisites;

using Psyche.Models.Mocks;

/// <summary>
/// Prerequisite that checks if a storylet quality meets specified bounds.
/// Example: Requires social_capital >= 10, or psychological_strain < 50.
/// </summary>
public class QualityRequirement : IPrerequisite
{
    /// <summary>The ID of the quality to check (e.g., "social_capital", "psychological_strain").</summary>
    public string QualityId { get; set; } = string.Empty;

    /// <summary>Minimum required value (inclusive). Null means no minimum.</summary>
    public int? MinValue { get; set; }

    /// <summary>Maximum allowed value (inclusive). Null means no maximum.</summary>
    public int? MaxValue { get; set; }

    /// <summary>Evaluates whether the character's quality meets the specified bounds.</summary>
    public bool IsMet(Character character)
    {
        var value = character.GetQualityValue(QualityId);

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
            return $"{QualityId} between {MinValue}-{MaxValue}";
        else if (MinValue.HasValue)
            return $"{QualityId} ≥ {MinValue}";
        else if (MaxValue.HasValue)
            return $"{QualityId} ≤ {MaxValue}";
        else
            return $"{QualityId} (any value)";
    }
}
