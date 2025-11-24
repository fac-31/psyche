using Psyche.Data;
using Psyche.Models;
using Psyche.Models.Mocks;
using Psyche.UI;

namespace Psyche;

/// <summary>
/// Main entry point for the Psyche application.
/// Implements a basic playable game loop where users interact with storylets.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        // Display welcome message
        ConsoleDisplay.ShowWelcome();

        // Initialize character with starting attributes
        var character = CreateStartingCharacter();

        // Load storylet repository
        var repository = LoadStoryletRepository();

        // Show initial character state
        ConsoleDisplay.ShowCharacterState(character);
        ConsoleDisplay.WaitForKeyPress();

        // Main game loop
        bool continueGame = true;
        while (continueGame)
        {
            // Get available storylets based on character's current state
            var availableStorylets = GetAvailableStorylets(repository, character);

            if (!availableStorylets.Any())
            {
                // No more storylets available - game ends
                ConsoleDisplay.ShowNoStoryletsAvailable();
                break;
            }

            // Select the highest priority storylet
            var storylet = availableStorylets.First();

            // Display the storylet
            Console.Clear();
            ConsoleDisplay.ShowStorylet(storylet);

            // Apply storylet-level effects (if any)
            foreach (var effect in storylet.Effects)
            {
                effect.Apply(character);
            }

            // Check if storylet has options
            if (storylet.HasChoices)
            {
                // Get available options for this character
                var availableOptions = storylet.GetAvailableOptions(character);

                if (!availableOptions.Any())
                {
                    // No options available despite storylet being available
                    // This shouldn't normally happen, but handle it gracefully
                    ConsoleDisplay.ShowMessage("No available options at this time.");
                    character.MarkStoryletPlayed(storylet.Id);
                    continue;
                }

                // Display options and get user choice
                ConsoleDisplay.ShowOptions(availableOptions);
                int choiceIndex = ConsoleInput.GetChoice(1, availableOptions.Count) - 1;

                var chosenOption = availableOptions[choiceIndex];

                // Display the result
                ConsoleDisplay.ShowOptionResult(chosenOption);

                // Apply option effects
                foreach (var effect in chosenOption.Effects)
                {
                    effect.Apply(character);
                }
            }
            else
            {
                // Legacy storylet with no options - just auto-apply effects
                ConsoleDisplay.WaitForKeyPress();
            }

            // Mark storylet as played
            character.MarkStoryletPlayed(storylet.Id);

            // Show updated character state
            ConsoleDisplay.ShowSeparator();
            ConsoleDisplay.ShowCharacterState(character);

            // Ask if user wants to continue
            continueGame = ConsoleInput.GetYesNo("Continue your journey?");
            Console.WriteLine();
        }

        // Goodbye message
        ConsoleDisplay.ShowMessage("Thank you for playing Psyche!");
    }

    /// <summary>Creates a character with starting attributes.</summary>
    private static Character CreateStartingCharacter()
    {
        var character = new Character();

        // Set starting core attributes
        character.Attributes.SelfAssurance = 50;
        character.Attributes.Compassion = 50;
        character.Attributes.Ambition = 50;
        character.Attributes.Drive = 50;
        character.Attributes.Discernment = 50;
        character.Attributes.Bravery = 50;

        // Set starting qualities
        character.Qualities["social_capital"] = 5;

        return character;
    }

    /// <summary>Loads the storylet repository from the Data/Storylets directory.</summary>
    private static IStoryletRepository LoadStoryletRepository()
    {
        // Find the Data/Storylets directory
        var dataDirectory = FindDataDirectory();
        return new JsonStoryletRepository(dataDirectory);
    }

    /// <summary>Gets all storylets that are available to the character.</summary>
    private static List<Storylet> GetAvailableStorylets(IStoryletRepository repository, Character character)
    {
        return repository.GetAll()
            .Where(s => !character.HasPlayedStorylet(s.Id)) // Haven't played yet
            .Where(s => s.Prerequisites.All(prereq => prereq.IsMet(character))) // All prerequisites met
            .OrderByDescending(s => s.Priority) // Highest priority first
            .ToList();
    }

    /// <summary>Finds the Data/Storylets directory by searching up from the current directory.</summary>
    private static string FindDataDirectory()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var searchDir = currentDir;

        // First, check if we're in a bin directory and go up to project root
        if (searchDir.Contains("bin"))
        {
            var binIndex = searchDir.IndexOf("bin");
            searchDir = searchDir.Substring(0, binIndex);
        }

        // Search for Data/Storylets directory
        for (int i = 0; i < 10; i++)
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

        // If not found, throw an error
        throw new DirectoryNotFoundException(
            $"Could not find Data/Storylets directory. Searched from: {currentDir}. " +
            $"Please ensure the application is run from the project directory.");
    }
}
