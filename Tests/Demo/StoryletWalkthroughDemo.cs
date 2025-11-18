namespace Psyche.Tests.Demo;

using Psyche.Data;
using Psyche.Models;
using Psyche.Models.Mocks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// Demonstration of the complete storylet choice flow.
/// Shows a character encountering a storylet, viewing options, choosing one, and experiencing the effects.
/// </summary>
public class StoryletWalkthroughDemo
{
    private readonly ITestOutputHelper _output;

    public StoryletWalkthroughDemo(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CompleteStoryletWalkthrough_FromEncounterToConsequence()
    {
        PrintHeader("STORYLET SYSTEM WALKTHROUGH");
        _output.WriteLine("This demo shows the complete flow of encountering a storylet,");
        _output.WriteLine("evaluating options, making a choice, and experiencing the consequences.");
        _output.WriteLine("");

        // ===== STEP 1: Initialize Character =====
        PrintStep(1, "Create Character");

        var character = new Character();
        character.Attributes.Compassion = 45;
        character.Attributes.Discernment = 55;
        character.Attributes.Bravery = 35;
        character.Qualities["social_capital"] = 8;
        character.Qualities["secrets_learned"] = 2;

        PrintCharacterState(character);

        // ===== STEP 2: Load Storylet Repository =====
        PrintStep(2, "Load Storylets from Repository");

        // Find the Data/Storylets directory relative to the project root
        var dataDirectory = FindDataDirectory();
        var repository = new JsonStoryletRepository(dataDirectory);
        var availableStorylets = repository.GetAll().ToList();

        _output.WriteLine($"Repository loaded {availableStorylets.Count} storylet(s):");
        foreach (var s in availableStorylets)
        {
            _output.WriteLine($"  • {s.Title} ({s.Id})");
        }
        _output.WriteLine("");

        // ===== STEP 3: Select a Storylet =====
        PrintStep(3, "Encounter a Storylet");

        var storylet = repository.GetById("first_encounter");
        storylet.Should().NotBeNull("storylet should exist in repository");

        _output.WriteLine($"═══════════════════════════════════════════════════════");
        _output.WriteLine($"  {storylet!.Title}");
        _output.WriteLine($"═══════════════════════════════════════════════════════");
        _output.WriteLine("");
        _output.WriteLine(storylet.Description);
        _output.WriteLine("");
        _output.WriteLine(WrapText(storylet.Content, 70));
        _output.WriteLine("");

        // Check storylet-level prerequisites
        var storyletAvailable = storylet.Prerequisites.All(prereq => prereq.IsMet(character));
        _output.WriteLine($"Storylet Prerequisites: {(storyletAvailable ? "✓ Met" : "✗ Not Met")}");
        if (storylet.Prerequisites.Any())
        {
            foreach (var prereq in storylet.Prerequisites)
            {
                _output.WriteLine($"  • {prereq.GetDisplayText()}");
            }
        }
        else
        {
            _output.WriteLine("  (None - always available)");
        }
        _output.WriteLine("");

        // ===== STEP 4: Evaluate Available Options =====
        PrintStep(4, "Evaluate Available Options");

        _output.WriteLine("The storylet presents several choices. Let's check which ones");
        _output.WriteLine("are available based on the character's current state:");
        _output.WriteLine("");

        var allOptions = storylet.Options;
        var availableOptions = storylet.GetAvailableOptions(character);

        _output.WriteLine($"Total Options: {allOptions.Count}");
        _output.WriteLine($"Available Options: {availableOptions.Count}");
        _output.WriteLine("");

        // Show all options with availability status
        foreach (var option in allOptions)
        {
            var isAvailable = availableOptions.Contains(option);
            var symbol = isAvailable ? "✓" : "✗";
            var status = isAvailable ? "AVAILABLE" : "LOCKED";

            _output.WriteLine($"[{symbol}] {option.Text} - {status}");
            _output.WriteLine($"    \"{option.Description}\"");

            // Show prerequisites
            if (option.Prerequisites.Any())
            {
                _output.WriteLine("    Prerequisites:");
                foreach (var prereq in option.Prerequisites)
                {
                    var isMet = prereq.IsMet(character);
                    var prereqSymbol = isMet ? "✓" : "✗";
                    _output.WriteLine($"      {prereqSymbol} {prereq.GetDisplayText()}");
                }
            }
            else
            {
                _output.WriteLine("    Prerequisites: (None)");
            }

            // Show what effects would happen
            if (option.Effects.Any())
            {
                _output.WriteLine("    Effects:");
                foreach (var effect in option.Effects)
                {
                    _output.WriteLine($"      → {effect.GetDisplayText()}");
                }
            }

            _output.WriteLine("");
        }

        // ===== STEP 5: Player Makes a Choice =====
        PrintStep(5, "Player Makes a Choice");

        availableOptions.Should().NotBeEmpty("at least one option should be available");

        // Choose the first available option
        var chosenOption = availableOptions.First();

        _output.WriteLine($"The player chooses: \"{chosenOption.Text}\"");
        _output.WriteLine("");
        _output.WriteLine($"═══════════════════════════════════════════════════════");
        _output.WriteLine(WrapText(chosenOption.ResultText, 70));
        _output.WriteLine($"═══════════════════════════════════════════════════════");
        _output.WriteLine("");

        // ===== STEP 6: Apply Storylet-Level Effects (if any) =====
        if (storylet.Effects.Any())
        {
            PrintStep(6, "Apply Storylet-Level Effects");
            _output.WriteLine("(These are automatic effects from viewing/entering the storylet)");
            _output.WriteLine("");

            foreach (var effect in storylet.Effects)
            {
                _output.WriteLine($"  → {effect.GetDisplayText()}");
                effect.Apply(character);
            }
            _output.WriteLine("");
        }

        // ===== STEP 7: Apply Choice Effects =====
        var stepNum = storylet.Effects.Any() ? 7 : 6;
        PrintStep(stepNum, "Apply Choice Consequences");

        _output.WriteLine($"Applying effects from \"{chosenOption.Text}\":");
        _output.WriteLine("");

        // Store before state for comparison
        var beforeCompassion = character.Attributes.Compassion;
        var beforeDiscernment = character.Attributes.Discernment;
        var beforeSocialCapital = character.GetQualityValue("social_capital");
        var beforeSecretsLearned = character.GetQualityValue("secrets_learned");
        var storyletsPlayedBefore = character.PlayedStoryletIds.Count;

        foreach (var effect in chosenOption.Effects)
        {
            _output.WriteLine($"  → {effect.GetDisplayText()}");
            effect.Apply(character);
        }
        _output.WriteLine("");

        // ===== STEP 8: Show Final State =====
        PrintStep(stepNum + 1, "Character State After Choice");

        _output.WriteLine("Changes:");
        _output.WriteLine("");

        // Show what changed
        if (character.Attributes.Compassion != beforeCompassion)
        {
            var delta = character.Attributes.Compassion - beforeCompassion;
            var sign = delta > 0 ? "+" : "";
            _output.WriteLine($"  • Compassion: {beforeCompassion} → {character.Attributes.Compassion} ({sign}{delta})");
        }

        if (character.Attributes.Discernment != beforeDiscernment)
        {
            var delta = character.Attributes.Discernment - beforeDiscernment;
            var sign = delta > 0 ? "+" : "";
            _output.WriteLine($"  • Discernment: {beforeDiscernment} → {character.Attributes.Discernment} ({sign}{delta})");
        }

        var afterSocialCapital = character.GetQualityValue("social_capital");
        if (afterSocialCapital != beforeSocialCapital)
        {
            var delta = afterSocialCapital - beforeSocialCapital;
            var sign = delta > 0 ? "+" : "";
            _output.WriteLine($"  • Social Capital: {beforeSocialCapital} → {afterSocialCapital} ({sign}{delta})");
        }

        var afterSecretsLearned = character.GetQualityValue("secrets_learned");
        if (afterSecretsLearned != beforeSecretsLearned)
        {
            var delta = afterSecretsLearned - beforeSecretsLearned;
            var sign = delta > 0 ? "+" : "";
            _output.WriteLine($"  • Secrets Learned: {beforeSecretsLearned} → {afterSecretsLearned} ({sign}{delta})");
        }

        if (character.PlayedStoryletIds.Count != storyletsPlayedBefore)
        {
            _output.WriteLine($"  • Storylets Unlocked: '{storylet.Id}' marked as played");
        }

        _output.WriteLine("");
        PrintCharacterState(character);

        // ===== STEP 9: Check What's Next =====
        PrintStep(stepNum + 2, "What Happens Next?");

        _output.WriteLine("The character's choices have consequences. Let's see what");
        _output.WriteLine("storylets might be available now:");
        _output.WriteLine("");

        // Check if any storylets require the one we just played
        var unlockedByChoice = availableStorylets
            .Where(s => s.Prerequisites.Any(p =>
                p is Psyche.Systems.Prerequisites.StoryletPlayedRequirement spr
                && spr.StoryletId == storylet.Id
                && spr.MustHavePlayed))
            .ToList();

        if (unlockedByChoice.Any())
        {
            _output.WriteLine("Storylets now unlocked:");
            foreach (var s in unlockedByChoice)
            {
                var nowAvailable = s.Prerequisites.All(p => p.IsMet(character));
                var symbol = nowAvailable ? "✓" : "◷";
                var status = nowAvailable ? "Available Now" : "Still Locked (other requirements)";
                _output.WriteLine($"  {symbol} {s.Title} - {status}");
            }
        }
        else
        {
            _output.WriteLine("No storylets directly unlocked by this choice.");
            _output.WriteLine("(But character attributes have changed, affecting future options)");
        }

        _output.WriteLine("");
        PrintFooter();

        // Verify the demo worked
        chosenOption.Effects.Should().NotBeEmpty("demo should show effects being applied");
    }

    private void PrintHeader(string title)
    {
        _output.WriteLine("");
        _output.WriteLine("╔" + new string('═', 78) + "╗");
        _output.WriteLine("║" + title.PadLeft(39 + title.Length / 2).PadRight(78) + "║");
        _output.WriteLine("╚" + new string('═', 78) + "╝");
        _output.WriteLine("");
    }

    private void PrintStep(int stepNumber, string title)
    {
        _output.WriteLine("");
        _output.WriteLine($"┌─ STEP {stepNumber}: {title}");
        _output.WriteLine($"└" + new string('─', 70));
        _output.WriteLine("");
    }

    private void PrintCharacterState(Character character)
    {
        _output.WriteLine("┌─ Current Character State");
        _output.WriteLine("│");
        _output.WriteLine("│  Core Attributes:");
        _output.WriteLine($"│    • Self-Assurance: {character.Attributes.SelfAssurance}");
        _output.WriteLine($"│    • Compassion:     {character.Attributes.Compassion}");
        _output.WriteLine($"│    • Ambition:       {character.Attributes.Ambition}");
        _output.WriteLine($"│    • Drive:          {character.Attributes.Drive}");
        _output.WriteLine($"│    • Discernment:    {character.Attributes.Discernment}");
        _output.WriteLine($"│    • Bravery:        {character.Attributes.Bravery}");

        if (character.Qualities.Any())
        {
            _output.WriteLine("│");
            _output.WriteLine("│  Qualities:");
            foreach (var quality in character.Qualities.OrderBy(q => q.Key))
            {
                _output.WriteLine($"│    • {quality.Key}: {quality.Value}");
            }
        }

        if (character.PlayedStoryletIds.Any())
        {
            _output.WriteLine("│");
            _output.WriteLine("│  Storylets Played:");
            foreach (var id in character.PlayedStoryletIds)
            {
                _output.WriteLine($"│    • {id}");
            }
        }

        _output.WriteLine("└" + new string('─', 70));
        _output.WriteLine("");
    }

    private void PrintFooter()
    {
        _output.WriteLine("╔" + new string('═', 78) + "╗");
        _output.WriteLine("║" + "END OF DEMONSTRATION".PadLeft(49).PadRight(78) + "║");
        _output.WriteLine("╚" + new string('═', 78) + "╝");
        _output.WriteLine("");
    }

    private string WrapText(string text, int maxWidth)
    {
        if (text.Length <= maxWidth)
            return text;

        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = "";

        foreach (var word in words)
        {
            if (currentLine.Length + word.Length + 1 > maxWidth)
            {
                if (currentLine.Length > 0)
                {
                    lines.Add(currentLine);
                    currentLine = "";
                }
            }

            if (currentLine.Length > 0)
                currentLine += " ";
            currentLine += word;
        }

        if (currentLine.Length > 0)
            lines.Add(currentLine);

        return string.Join("\n", lines);
    }

    private string FindDataDirectory()
    {
        // Start from current directory and search up for Data/Storylets
        var currentDir = Directory.GetCurrentDirectory();
        var searchDir = currentDir;

        for (int i = 0; i < 10; i++) // Search up to 10 levels
        {
            var dataPath = Path.Combine(searchDir, "Data", "Storylets");

            if (Directory.Exists(dataPath))
            {
                return dataPath;
            }

            var parentDir = Directory.GetParent(searchDir);
            if (parentDir == null)
                break;
            searchDir = parentDir.FullName;
        }

        // If not found, throw a helpful error
        throw new DirectoryNotFoundException(
            $"Could not find Data/Storylets directory. Searched from: {currentDir}");
    }
}
