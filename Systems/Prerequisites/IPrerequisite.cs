namespace Psyche.Systems.Prerequisites;

using Psyche.Models.Mocks;

/// <summary>Represents a condition that must be met for a storylet to be available.</summary>
public interface IPrerequisite
{
    /// <summary>Evaluates whether this prerequisite is met for the given character.</summary>
    /// <param name="character">The character to evaluate against.</param>
    /// <returns>True if the prerequisite is met, false otherwise.</returns>
    bool IsMet(Character character);

    /// <summary>
    /// Gets a human-readable description of this prerequisite.
    /// Used for displaying requirements to the player.
    /// </summary>
    /// <returns>Display text for UI presentation.</returns>
    string GetDisplayText();
}
