namespace Psyche.Systems.CharacterCreation;

public class ReformerArchetype : ICharacterArchetype
{
    public string Name => "The Reformer";

    public void ApplyModifiers(Character character)
    {
        character.Discernment += 15;
        character.Compassion += 10;
        character.SelfAssurance -= 5;
    }

    public bool CheckWinCondition(Character character)
    {
        // Win Condition: Compassion >= 70 AND Discernment >= 70 AND psychological_strain < 30
        
        // Note: Psychological strain is a quality, not a core attribute. 
        // We need to implement the Quality system to fully check this.
        // For now, we check the core attributes.
        
        return character.Compassion >= 70 
            && character.Discernment >= 70
            && character.SelfAssurance < 30;
    }
}
