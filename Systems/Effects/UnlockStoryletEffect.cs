namespace Psyche.Systems.Effects;

using Psyche.Models.Mocks;

/// <summary>
/// Effect that marks a storylet as played.
/// Used to unlock future storylets that require specific prerequisite storylets.
/// Example: Mark "chapter_1_complete" as played to enable "chapter_2_start".
/// </summary>
public class UnlockStoryletEffect : IEffect
{
    /// <summary>The ID of the storylet to mark as played.</summary>
    public string StoryletId { get; set; } = string.Empty;

    /// <summary>Marks the storylet as played on the character.</summary>
    public void Apply(Character character)
    {
        character.MarkStoryletPlayed(StoryletId);
    }

    /// <summary>Gets a human-readable description of this effect.</summary>
    public string GetDisplayText()
    {
        return $"Unlocks: {StoryletId}";
    }
}
