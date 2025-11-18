namespace Psyche.Data;

using System.Text.Json;
using Psyche.Models;
using Psyche.Data.Json;

/// <summary>Loads storylets from .jsonc files in a specified directory.</summary>
public class JsonStoryletRepository : IStoryletRepository
{
    private readonly string _dataDirectory;
    private readonly Dictionary<string, Storylet> _storylets;

    /// <summary>Creates a new JSON storylet repository.</summary>
    /// <param name="dataDirectory">Directory containing .jsonc storylet files.</param>
    public JsonStoryletRepository(string dataDirectory)
    {
        _dataDirectory = dataDirectory;
        _storylets = new Dictionary<string, Storylet>();
        LoadAllStorylets();
    }

    /// <summary>Gets a storylet by its unique ID.</summary>
    public Storylet? GetById(string id)
    {
        return _storylets.TryGetValue(id, out var storylet) ? storylet : null;
    }

    /// <summary>Gets all storylets.</summary>
    public IEnumerable<Storylet> GetAll()
    {
        return _storylets.Values;
    }

    /// <summary>Gets storylets by category.</summary>
    public IEnumerable<Storylet> GetByCategory(string category)
    {
        return _storylets.Values.Where(s => s.Category == category);
    }

    /// <summary>Gets storylets that have any of the specified tags.</summary>
    public IEnumerable<Storylet> GetByTags(params string[] tags)
    {
        return _storylets.Values.Where(s => s.Tags.Any(tag => tags.Contains(tag)));
    }

    /// <summary>Adds or updates a storylet in the repository.</summary>
    public void Save(Storylet storylet)
    {
        // Validate storylet before saving
        var validation = storylet.Validate();
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(
                $"Cannot save invalid storylet '{storylet.Id}': {string.Join(", ", validation.Errors)}");
        }

        // Update in-memory cache
        _storylets[storylet.Id] = storylet;

        // Save to file
        SaveStoryletFile(storylet);
    }

    /// <summary>Removes a storylet by ID.</summary>
    public bool Delete(string id)
    {
        if (!_storylets.ContainsKey(id))
            return false;

        // Remove from cache
        _storylets.Remove(id);

        // Delete file
        var filePath = Path.Combine(_dataDirectory, $"{id}.jsonc");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return true;
    }

    /// <summary>Reloads all storylets from the data source.</summary>
    public void Reload()
    {
        _storylets.Clear();
        LoadAllStorylets();
    }

    /// <summary>Loads all storylet files from the data directory.</summary>
    private void LoadAllStorylets()
    {
        if (!Directory.Exists(_dataDirectory))
        {
            Console.WriteLine($"Warning: Storylet directory '{_dataDirectory}' does not exist. Creating it.");
            Directory.CreateDirectory(_dataDirectory);
            return;
        }

        var jsonFiles = Directory.GetFiles(_dataDirectory, "*.jsonc");

        foreach (var filePath in jsonFiles)
        {
            try
            {
                LoadStoryletFile(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading storylet file '{Path.GetFileName(filePath)}': {ex.Message}");
                // Continue loading other files
            }
        }

        Console.WriteLine($"Loaded {_storylets.Count} storylet(s) from '{_dataDirectory}'");
    }

    /// <summary>Loads a single storylet file.</summary>
    private void LoadStoryletFile(string filePath)
    {
        var jsonc = File.ReadAllText(filePath);
        var dto = JsonSerializer.Deserialize<StoryletDto>(jsonc, StoryletJsonConverter.JsonOptions);

        if (dto == null)
        {
            throw new InvalidOperationException($"Failed to deserialize storylet from '{filePath}'");
        }

        var storylet = StoryletJsonConverter.FromDto(dto);

        // Validate storylet
        var validation = storylet.Validate();
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(
                $"Invalid storylet in '{filePath}': {string.Join(", ", validation.Errors)}");
        }

        _storylets[storylet.Id] = storylet;
    }

    /// <summary>Saves a storylet to a file.</summary>
    private void SaveStoryletFile(Storylet storylet)
    {
        if (!Directory.Exists(_dataDirectory))
        {
            Directory.CreateDirectory(_dataDirectory);
        }

        var dto = StoryletJsonConverter.ToDto(storylet);
        var json = JsonSerializer.Serialize(dto, StoryletJsonConverter.JsonOptions);

        var filePath = Path.Combine(_dataDirectory, $"{storylet.Id}.jsonc");
        File.WriteAllText(filePath, json);
    }
}
