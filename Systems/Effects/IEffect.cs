namespace Psyche.Systems.Effects;

using Psyche.Models.Mocks;

/// <summary>Represents a modification to character state when a storylet is executed.</summary>
public interface IEffect
{
    /// <summary>
    /// Applies this effect to the given character.
    /// This may modify attributes, qualities, or unlock new storylets.
    /// </summary>
    /// <param name="character">The character to modify.</param>
    void Apply(Character character);

    /// <summary>
    /// Gets a human-readable description of this effect.
    /// Used for displaying consequences to the player.
    /// </summary>
    /// <returns>Display text for UI presentation.</returns>
    string GetDisplayText();
}
