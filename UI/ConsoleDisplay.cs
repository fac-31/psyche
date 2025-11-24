namespace Psyche.UI;

using Psyche.Models;
using Psyche.Models.Mocks;

/// <summary>
/// Handles displaying content to the user in the console.
/// </summary>
public static class ConsoleDisplay
{
    /// <summary>Displays the game title and welcome message.</summary>
    public static void ShowWelcome()
    {
        Console.Clear();
        Console.WriteLine("╔" + new string('═', 78) + "╗");
        Console.WriteLine("║" + "PSYCHE".PadLeft(42).PadRight(78) + "║");
        Console.WriteLine("║" + "A Quality-Based Narrative Experience".PadLeft(57).PadRight(78) + "║");
        Console.WriteLine("╚" + new string('═', 78) + "╝");
        Console.WriteLine();
        Console.WriteLine("Welcome to Psyche, where your choices shape your character's journey.");
        Console.WriteLine();
    }

    /// <summary>Displays the character's current state.</summary>
    /// <param name="character">The character to display.</param>
    public static void ShowCharacterState(Character character)
    {
        Console.WriteLine();
        Console.WriteLine("┌─ Your Character");
        Console.WriteLine("│");
        Console.WriteLine("│  Core Attributes:");
        Console.WriteLine($"│    Self-Assurance: {character.Attributes.SelfAssurance,3}  |  Compassion:  {character.Attributes.Compassion,3}");
        Console.WriteLine($"│    Ambition:       {character.Attributes.Ambition,3}  |  Drive:       {character.Attributes.Drive,3}");
        Console.WriteLine($"│    Discernment:    {character.Attributes.Discernment,3}  |  Bravery:     {character.Attributes.Bravery,3}");

        if (character.Qualities.Any())
        {
            Console.WriteLine("│");
            Console.WriteLine("│  Qualities:");
            foreach (var quality in character.Qualities.OrderBy(q => q.Key))
            {
                Console.WriteLine($"│    {FormatQualityName(quality.Key)}: {quality.Value}");
            }
        }

        Console.WriteLine("└" + new string('─', 70));
        Console.WriteLine();
    }

    /// <summary>Displays available storylets for the user to choose from.</summary>
    /// <param name="storylets">The list of available storylets.</param>
    /// <param name="repository">Repository to look up previous storylet titles.</param>
    public static void ShowStoryletChoices(List<Storylet> storylets, Data.IStoryletRepository repository)
    {
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
        Console.WriteLine("  Available Storylets");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine("Choose a storylet to explore:");
        Console.WriteLine();

        for (int i = 0; i < storylets.Count; i++)
        {
            var storylet = storylets[i];
            Console.WriteLine($"{i + 1}. {storylet.Title}");

            // Show previous storylet in chain if any
            var previousId = storylet.GetPreviousStoryletId();
            if (!string.IsNullOrEmpty(previousId))
            {
                var previousStorylet = repository.GetById(previousId);
                if (previousStorylet != null)
                {
                    Console.WriteLine($"   (Continued from \"{previousStorylet.Title}\")");
                }
            }

            // Show unlock conditions
            var prerequisites = storylet.GetDisplayablePrerequisites();
            if (prerequisites.Any())
            {
                Console.WriteLine($"   Unlock conditions: {string.Join(", ", prerequisites)}");
            }

            Console.WriteLine();
        }
    }

    /// <summary>Displays a storylet with its title, description, and content.</summary>
    /// <param name="storylet">The storylet to display.</param>
    public static void ShowStorylet(Storylet storylet)
    {
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
        Console.WriteLine($"  {storylet.Title}");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine(WrapText(storylet.Content, 78));
        Console.WriteLine();
    }

    /// <summary>Displays available options for a storylet.</summary>
    /// <param name="options">The list of available options.</param>
    public static void ShowOptions(List<StoryletOption> options)
    {
        Console.WriteLine("What do you do?");
        Console.WriteLine();

        for (int i = 0; i < options.Count; i++)
        {
            var option = options[i];
            Console.WriteLine($"  [{i + 1}] {option.Text}");
            Console.WriteLine($"      {option.Description}");
            Console.WriteLine();
        }
    }

    /// <summary>Displays the result of choosing an option.</summary>
    /// <param name="option">The chosen option.</param>
    public static void ShowOptionResult(StoryletOption option)
    {
        Console.WriteLine();
        Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");
        Console.WriteLine(WrapText(option.ResultText, 78));
        Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");
        Console.WriteLine();
    }

    /// <summary>Displays a message indicating no storylets are available.</summary>
    public static void ShowNoStoryletsAvailable()
    {
        Console.WriteLine();
        Console.WriteLine("There are no storylets available at this time.");
        Console.WriteLine("Your journey has reached its current end.");
        Console.WriteLine();
    }

    /// <summary>Displays a separator line.</summary>
    public static void ShowSeparator()
    {
        Console.WriteLine();
        Console.WriteLine(new string('─', 78));
        Console.WriteLine();
    }

    /// <summary>Displays a message to the user.</summary>
    /// <param name="message">The message to display.</param>
    public static void ShowMessage(string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();
    }

    /// <summary>Displays a prompt and waits for the user to press a key.</summary>
    /// <param name="prompt">The prompt to display. Defaults to "Press any key to continue..."</param>
    public static void WaitForKeyPress(string prompt = "Press any key to continue...")
    {
        Console.WriteLine();
        Console.WriteLine(prompt);
        Console.ReadKey(true);
    }

    /// <summary>Wraps text to fit within a specified width.</summary>
    /// <param name="text">The text to wrap.</param>
    /// <param name="maxWidth">The maximum width in characters.</param>
    /// <returns>The wrapped text.</returns>
    private static string WrapText(string text, int maxWidth)
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

    /// <summary>Formats a quality name for display (converts snake_case to Title Case).</summary>
    /// <param name="qualityId">The quality ID to format.</param>
    /// <returns>The formatted quality name.</returns>
    private static string FormatQualityName(string qualityId)
    {
        return string.Join(" ", qualityId.Split('_')
            .Select(word => char.ToUpper(word[0]) + word.Substring(1)));
    }
}
