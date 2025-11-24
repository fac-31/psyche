namespace Psyche.Systems.CharacterCreation;

public static class ArchetypeFactory
{
    private static readonly List<ICharacterArchetype> _archetypes = new()
    {
        new ReformerArchetype(),
        new HelperArchetype(),
        new AchieverArchetype()
        // TODO: Add other archetypes as they are implemented
    };

    public static ICharacterArchetype GetRandomArchetype()
    {
        var random = new Random();
        int index = random.Next(_archetypes.Count);
        return _archetypes[index];
    }

    public static IEnumerable<ICharacterArchetype> GetAllArchetypes()
    {
        return _archetypes;
    }
}
