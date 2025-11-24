namespace Psyche.Systems.CharacterCreation;

public interface ICharacterArchetype
{
    // The name of the archetype (e.g., "The Reformer")
    string Name { get; }
    
    // Applies the +20/-20 modifiers to the character
    void ApplyModifiers(Character character);
    
    // Checks if the character has met the win condition for this archetype
    bool CheckWinCondition(Character character);
}