namespace Psyche.Systems.CharacterCreation;

public interface ICharacterBuilder
{
    ICharacterBuilder WithName(string name);

    ICharacterBuilder WithArchetype(ICharacterArchetype archetype);

    ICharacterBuilder WithRandomDrive();

    Character Build();
}