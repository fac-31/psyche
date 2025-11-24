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
        return character.Compassion >= 70 
            && character.Discernment >= 70
            && character.SelfAssurance < 30;
    }
}
