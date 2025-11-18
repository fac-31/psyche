namespace Psyche.Models;

/// <summary>
/// Result of validating a storylet configuration.
/// </summary>
public class StoryletValidationResult
{
    /// <summary>True if the storylet passed all validation checks.</summary>
    public bool IsValid { get; set; }

    /// <summary>List of validation error messages (empty if valid).</summary>
    public List<string> Errors { get; set; } = new();
}
