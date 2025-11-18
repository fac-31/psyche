namespace Psyche.Tests;

using Psyche.Models.Mocks;
using Psyche.Systems.Prerequisites;
using Psyche.Systems.Effects;
using Xunit.Abstractions;

/// <summary>
/// Helper methods for adding detailed output to tests, making the storylet system visible.
/// </summary>
public static class TestOutputHelpers
{
    /// <summary>Logs the current state of a character (attributes and qualities).</summary>
    public static void LogCharacterState(ITestOutputHelper output, Character character, string header = "Character State")
    {
        output.WriteLine($"\n=== {header} ===");
        output.WriteLine("Attributes:");
        output.WriteLine($"  - Self-Assurance: {character.Attributes.SelfAssurance}");
        output.WriteLine($"  - Compassion: {character.Attributes.Compassion}");
        output.WriteLine($"  - Ambition: {character.Attributes.Ambition}");
        output.WriteLine($"  - Drive: {character.Attributes.Drive}");
        output.WriteLine($"  - Discernment: {character.Attributes.Discernment}");
        output.WriteLine($"  - Bravery: {character.Attributes.Bravery}");

        if (character.Qualities.Any())
        {
            output.WriteLine("Qualities:");
            foreach (var quality in character.Qualities)
            {
                output.WriteLine($"  - {quality.Key}: {quality.Value}");
            }
        }

        if (character.PlayedStoryletIds.Any())
        {
            output.WriteLine("Played Storylets:");
            foreach (var storyletId in character.PlayedStoryletIds)
            {
                output.WriteLine($"  - {storyletId}");
            }
        }
        output.WriteLine("");
    }

    /// <summary>Logs a prerequisite check with result.</summary>
    public static void LogPrerequisiteCheck(ITestOutputHelper output, IPrerequisite prerequisite, Character character)
    {
        var isMet = prerequisite.IsMet(character);
        var symbol = isMet ? "✓" : "✗";
        var result = isMet ? "MET" : "NOT MET";

        output.WriteLine($"\n--- Prerequisite Check ---");
        output.WriteLine($"Requirement: {prerequisite.GetDisplayText()}");
        output.WriteLine($"Result: {symbol} {result}");
    }

    /// <summary>Logs an effect being applied.</summary>
    public static void LogEffectApplication(ITestOutputHelper output, IEffect effect, Character characterBefore, Character characterAfter)
    {
        output.WriteLine($"\n--- Effect Applied ---");
        output.WriteLine($"Effect: {effect.GetDisplayText()}");
        output.WriteLine("Changes:");

        // Compare attributes
        CompareAttribute(output, "Self-Assurance", characterBefore.Attributes.SelfAssurance, characterAfter.Attributes.SelfAssurance);
        CompareAttribute(output, "Compassion", characterBefore.Attributes.Compassion, characterAfter.Attributes.Compassion);
        CompareAttribute(output, "Ambition", characterBefore.Attributes.Ambition, characterAfter.Attributes.Ambition);
        CompareAttribute(output, "Drive", characterBefore.Attributes.Drive, characterAfter.Attributes.Drive);
        CompareAttribute(output, "Discernment", characterBefore.Attributes.Discernment, characterAfter.Attributes.Discernment);
        CompareAttribute(output, "Bravery", characterBefore.Attributes.Bravery, characterAfter.Attributes.Bravery);

        // Compare qualities
        var allQualities = characterBefore.Qualities.Keys.Union(characterAfter.Qualities.Keys);
        foreach (var qualityId in allQualities)
        {
            var before = characterBefore.GetQualityValue(qualityId);
            var after = characterAfter.GetQualityValue(qualityId);
            if (before != after)
            {
                var delta = after - before;
                var sign = delta >= 0 ? "+" : "";
                output.WriteLine($"  - {qualityId}: {before} → {after} ({sign}{delta})");
            }
        }

        // Compare played storylets
        var newStorylets = characterAfter.PlayedStoryletIds.Except(characterBefore.PlayedStoryletIds);
        foreach (var storyletId in newStorylets)
        {
            output.WriteLine($"  - Unlocked: {storyletId}");
        }
    }

    /// <summary>Logs a scenario walkthrough (multiple steps).</summary>
    public static void LogScenario(ITestOutputHelper output, string scenarioName)
    {
        output.WriteLine("\n" + new string('=', 60));
        output.WriteLine($"SCENARIO: {scenarioName}");
        output.WriteLine(new string('=', 60));
    }

    /// <summary>Logs a step in a scenario.</summary>
    public static void LogStep(ITestOutputHelper output, int stepNumber, string description)
    {
        output.WriteLine($"\n[Step {stepNumber}] {description}");
    }

    private static void CompareAttribute(ITestOutputHelper output, string name, int before, int after)
    {
        if (before != after)
        {
            var delta = after - before;
            var sign = delta >= 0 ? "+" : "";
            output.WriteLine($"  - {name}: {before} → {after} ({sign}{delta})");
        }
    }

    private static Character CloneCharacter(Character original)
    {
        var clone = new Character
        {
            Attributes = new CoreAttributes
            {
                SelfAssurance = original.Attributes.SelfAssurance,
                Compassion = original.Attributes.Compassion,
                Ambition = original.Attributes.Ambition,
                Drive = original.Attributes.Drive,
                Discernment = original.Attributes.Discernment,
                Bravery = original.Attributes.Bravery
            },
            Qualities = new Dictionary<string, int>(original.Qualities),
            PlayedStoryletIds = new HashSet<string>(original.PlayedStoryletIds)
        };
        return clone;
    }
}
