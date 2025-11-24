namespace Psyche.Models.Mocks;

/// <summary>
/// MOCK CLASS - Temporary implementation until M1 (Quality System) is complete.
/// Represents a game character with core attributes and storylet qualities.
/// Replace with actual implementation from M1 when available.
/// </summary>
public class Character
{
    /// <summary>The character's core personality attributes (6 scales from 0-100).</summary>
    public CoreAttributes Attributes { get; set; } = new();

    /// <summary>
    /// Storylet qualities (Progress, Menace, Resource, Metric types).
    /// Keys are quality IDs (e.g., "social_capital", "psychological_strain").
    /// Values are integer quality levels.
    /// </summary>
    public Dictionary<string, int> Qualities { get; set; } = new();

    /// <summary>Track which storylets have been played for sequencing and unlocking.</summary>
    public HashSet<string> PlayedStoryletIds { get; set; } = new();

    /// <summary>Gets a quality value, returning 0 if not present.</summary>
    /// <param name="qualityId">The ID of the quality to retrieve.</param>
    /// <returns>The quality value, or 0 if the quality doesn't exist.</returns>
    public int GetQualityValue(string qualityId)
    {
        return Qualities.TryGetValue(qualityId, out var value) ? value : 0;
    }

    /// <summary>Modifies a quality value, creating it if it doesn't exist.</summary>
    /// <param name="qualityId">The ID of the quality to modify.</param>
    /// <param name="delta">The amount to add (positive) or subtract (negative).</param>
    public void ModifyQuality(string qualityId, int delta)
    {
        var currentValue = GetQualityValue(qualityId);
        Qualities[qualityId] = currentValue + delta;
    }

    /// <summary>Checks if a storylet has been played.</summary>
    /// <param name="storyletId">The ID of the storylet to check.</param>
    /// <returns>True if the storylet has been played, false otherwise.</returns>
    public bool HasPlayedStorylet(string storyletId)
    {
        return PlayedStoryletIds.Contains(storyletId);
    }

    /// <summary>Marks a storylet as played.</summary>
    /// <param name="storyletId">The ID of the storylet to mark as played.</param>
    public void MarkStoryletPlayed(string storyletId)
    {
        PlayedStoryletIds.Add(storyletId);
    }
}
