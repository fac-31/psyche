using Psyche.Systems.CharacterCreation;

Console.WriteLine("=== Character Creation Test ===");

var builder = new CharacterBuilder();
var character = builder
    .WithName("Test Subject")
    .WithRandomDrive()
    .Build();

Console.WriteLine($"Created Character: {character.Name}");
Console.WriteLine($"Archetype: {character.Archetype.Name}");
Console.WriteLine("Attributes:");
Console.WriteLine($"  Self-Assurance: {character.SelfAssurance}");
Console.WriteLine($"  Compassion:     {character.Compassion}");
Console.WriteLine($"  Ambition:       {character.Ambition}");
Console.WriteLine($"  Drive:          {character.Drive}");
Console.WriteLine($"  Discernment:    {character.Discernment}");
Console.WriteLine($"  Bravery:        {character.Bravery}");

Console.WriteLine("=== Test Complete ===");
