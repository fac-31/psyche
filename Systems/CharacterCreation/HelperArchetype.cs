namespace Psyche.Systems.CharacterCreation;

public class HelperArchetype : ICharacterArchetype
{
    public string Name => "The Helper";

    public void ApplyModifiers(Character character)
    {
        character.Compassion += 15;
        character.Drive += 10;
        character.Bravery -= 5;
    }

    public bool CheckWinCondition(Character character)
    {
        // Win Condition: Compassion >= 60 AND Compassion < 80 AND social_capital >= 80 AND Self-Assurance >= 60
        
        return character.Compassion >= 60 
            && character.Compassion < 80
            && character.SelfAssurance >= 60;
            // && character.Qualities["social_capital"] >= 80;
    }
}
