namespace Psyche.Systems.CharacterCreation;

public class AchieverArchetype : ICharacterArchetype
{
    public string Name => "The Achiever";

    public void ApplyModifiers(Character character)
    {
        character.Ambition += 15;
        character.Drive += 15;
        character.Compassion -= 10;
    }

    public bool CheckWinCondition(Character character)
    {
        // Win Condition: main_story_progress ≥ 90 AND social_capital ≥ 70 AND Self-Assurance ≥ 70
        
        return character.SelfAssurance >= 70
            && character.Drive >= 90
            && character.Compassion >= 70;
    }
}
