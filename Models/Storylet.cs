namespace Psyche.Models;

using Psyche.Systems.Prerequisites;
using Psyche.Systems.Effects;
using Psyche.Models.Mocks;

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

    /// <summary>
    /// Changes applied to character state when storylet is executed.
    /// - If storylet has no options: effects are applied automatically
    /// - If storylet has options: effects are applied when storylet is viewed/entered
    /// Individual option effects are applied when that option is chosen.
    /// </summary>
    public List<IEffect> Effects { get; set; } = new();

    /// <summary>
    /// Available choices within this storylet.
    /// If empty, storylet uses legacy behavior (automatic effect application).
    /// If populated, player must choose one option.
    /// </summary>
    public List<StoryletOption> Options { get; set; } = new();

    /// <summary>Display priority (higher = shown first). Default: 10.</summary>
    public int Priority { get; set; } = 10;

    /// <summary>Category for organizing storylets (e.g., "exploration", "combat", "social").</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Tags for filtering and searching.</summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>Determines if this storylet has choices or uses legacy auto-execution.</summary>
    public bool HasChoices => Options.Any();

    /// <summary>Gets the ID of the previous storylet in the chain (if any).</summary>
    /// <returns>The storylet ID that must have been played before this one, or null if none.</returns>
    public string? GetPreviousStoryletId()
    {
        var requirement = Prerequisites
            .OfType<StoryletPlayedRequirement>()
            .FirstOrDefault(r => r.MustHavePlayed);
        return requirement?.StoryletId;
    }

    /// <summary>Gets a list of non-storylet prerequisites for display.</summary>
    /// <returns>List of requirement display texts.</returns>
    public List<string> GetDisplayablePrerequisites()
    {
        return Prerequisites
            .Where(p => p is not StoryletPlayedRequirement)
            .Select(p => p.GetDisplayText())
            .ToList();
    }

    /// <summary>Gets available options for the given character (filters by prerequisites).</summary>
    /// <param name="character">The character to check prerequisites against.</param>
    /// <returns>List of available options, ordered by priority (descending).</returns>
    public List<StoryletOption> GetAvailableOptions(Character character)
    {
        return Options
            .Where(option => option.Prerequisites.All(prereq => prereq.IsMet(character)))
            .OrderByDescending(option => option.Priority)
            .ToList();
    }

    /// <summary>Validates that the storylet is properly configured.</summary>
    /// <returns>Validation result with any error messages.</returns>
    public StoryletValidationResult Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Id))
            errors.Add("Storylet Id cannot be empty");
        if (string.IsNullOrWhiteSpace(Title))
            errors.Add("Storylet Title cannot be empty");
        // Option validation: if Options exist, validate each one
        if (Options.Any())
        {
            for (int i = 0; i < Options.Count; i++)
            {
                var option = Options[i];
                if (string.IsNullOrWhiteSpace(option.Id))
                    errors.Add($"Option {i} has empty Id");
                if (string.IsNullOrWhiteSpace(option.Text))
                    errors.Add($"Option {i} ({option.Id}) has empty Text");
            }

            // Check for duplicate option IDs
            var duplicateIds = Options
                .GroupBy(o => o.Id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var dupId in duplicateIds)
                errors.Add($"Duplicate option Id: {dupId}");
        }

        return new StoryletValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
}
