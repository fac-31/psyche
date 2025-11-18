namespace Psyche.Systems.Prerequisites;

using Psyche.Models.Mocks;

/// <summary>
/// Prerequisite that checks if a storylet has been played.
/// Useful for sequencing storylets and preventing repeats.
/// Example: Requires "intro_001" to have been played, or "ending_good" to NOT have been played.
/// </summary>
public class StoryletPlayedRequirement : IPrerequisite
{
    /// <summary>The ID of the storylet to check.</summary>
    public string StoryletId { get; set; } = string.Empty;

    /// <summary>True if the storylet must have been played, false if it must NOT have been played.</summary>
    public bool MustHavePlayed { get; set; } = true;

    /// <summary>Evaluates whether the storylet played requirement is met.</summary>
    public bool IsMet(Character character)
    {
        var hasPlayed = character.HasPlayedStorylet(StoryletId);
        return MustHavePlayed ? hasPlayed : !hasPlayed;
    }

    /// <summary>Gets a human-readable description of this requirement.</summary>
    public string GetDisplayText()
    {
        return MustHavePlayed
            ? $"Requires: {StoryletId} played"
            : $"Requires: {StoryletId} not played";
    }
}
