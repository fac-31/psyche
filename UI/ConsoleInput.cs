namespace Psyche.UI;

/// <summary>
/// Handles getting input from the user in the console.
/// </summary>
public static class ConsoleInput
{
    /// <summary>Gets a choice from the user within a valid range.</summary>
    /// <param name="minOption">The minimum valid option number (inclusive).</param>
    /// <param name="maxOption">The maximum valid option number (inclusive).</param>
    /// <param name="prompt">Optional prompt to display. Defaults to "Your choice: ".</param>
    /// <returns>The user's validated choice.</returns>
    public static int GetChoice(int minOption, int maxOption, string prompt = "Your choice: ")
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (int.TryParse(input, out int choice) && choice >= minOption && choice <= maxOption)
            {
                return choice;
            }

            Console.WriteLine($"Invalid choice. Please enter a number between {minOption} and {maxOption}.");
        }
    }

    /// <summary>Asks the user a yes/no question.</summary>
    /// <param name="question">The question to ask.</param>
    /// <returns>True if the user answered yes, false if no.</returns>
    public static bool GetYesNo(string question)
    {
        while (true)
        {
            Console.Write($"{question} (y/n): ");
            var input = Console.ReadLine()?.Trim().ToLower();

            if (input == "y" || input == "yes")
                return true;
            if (input == "n" || input == "no")
                return false;

            Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
        }
    }

    /// <summary>Gets a string input from the user.</summary>
    /// <param name="prompt">The prompt to display.</param>
    /// <param name="allowEmpty">Whether to allow empty input. Defaults to false.</param>
    /// <returns>The user's input string.</returns>
    public static string GetString(string prompt, bool allowEmpty = false)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(input) || allowEmpty)
                return input;

            Console.WriteLine("Input cannot be empty. Please try again.");
        }
    }

    /// <summary>Prompts user to select from a menu of options.</summary>
    /// <param name="options">The list of option texts to display.</param>
    /// <param name="title">Optional title for the menu.</param>
    /// <returns>The zero-based index of the selected option.</returns>
    public static int GetMenuChoice(List<string> options, string? title = null)
    {
        if (title != null)
        {
            Console.WriteLine();
            Console.WriteLine(title);
            Console.WriteLine();
        }

        for (int i = 0; i < options.Count; i++)
        {
            Console.WriteLine($"  [{i + 1}] {options[i]}");
        }

        Console.WriteLine();
        return GetChoice(1, options.Count) - 1; // Convert to zero-based index
    }
}
