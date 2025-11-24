namespace Psyche.Systems.CharacterCreation;
using Psyche;



public class Character
{
    public string Name { get; set; }
    public ICharacterArchetype Archetype { get; set; }
    
    public int SelfAssurance { get; set; }
    public int Compassion { get; set; }
    public int Ambition { get; set; }
    public int Drive { get; set; }
    public int Discernment { get; set; }
    public int Bravery { get; set; }
}

public class CharacterBuilder : ICharacterBuilder
{
    private Character _character;

    public CharacterBuilder()
    {
        _character = new Character();
        InitializeDefaultAttributes();
    }

    private void InitializeDefaultAttributes()
    {
        // All attributes start at 50 (Balanced)
        _character.SelfAssurance = 50;
        _character.Compassion = 50;
        _character.Ambition = 50;
        _character.Drive = 50;
        _character.Discernment = 50;
        _character.Bravery = 50;
    }

    public ICharacterBuilder WithName(string name)
    {
        _character.Name = name;
        return this;
    }

    public ICharacterBuilder WithArchetype(ICharacterArchetype archetype)
    {
        _character.Archetype = archetype;
        // Apply the archetype's modifiers immediately
        archetype.ApplyModifiers(_character);
        return this;
    }

    public ICharacterBuilder WithRandomDrive()
    {
        var randomArchetype = ArchetypeFactory.GetRandomArchetype();
        return WithArchetype(randomArchetype);
    }

    public Character Build()
    {
        ValidateCharacter();
        return _character;
    }

    private void ValidateCharacter()
    {
        if (string.IsNullOrWhiteSpace(_character.Name))
            throw new InvalidOperationException("Character must have a name.");
            
        if (_character.Archetype == null)
            throw new InvalidOperationException("Character must have an archetype selected.");
    }
}

