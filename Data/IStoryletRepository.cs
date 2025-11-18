namespace Psyche.Data;

using Psyche.Models;

/// <summary>
/// Repository interface for storylet storage and retrieval.
/// Abstracts the data source (JSON files, database, etc.).
/// </summary>
public interface IStoryletRepository
{
    /// <summary>
    /// Gets a storylet by its unique ID.
    /// </summary>
    /// <param name="id">The storylet ID to retrieve.</param>
    /// <returns>The storylet if found; otherwise, null.</returns>
    Storylet? GetById(string id);

    /// <summary>
    /// Gets all storylets.
    /// </summary>
    /// <returns>Collection of all storylets in the repository.</returns>
    IEnumerable<Storylet> GetAll();

    /// <summary>
    /// Gets storylets by category.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <returns>Collection of storylets in the specified category.</returns>
    IEnumerable<Storylet> GetByCategory(string category);

    /// <summary>
    /// Gets storylets that have any of the specified tags.
    /// </summary>
    /// <param name="tags">Tags to search for.</param>
    /// <returns>Collection of storylets matching any of the tags.</returns>
    IEnumerable<Storylet> GetByTags(params string[] tags);

    /// <summary>
    /// Adds or updates a storylet in the repository.
    /// </summary>
    /// <param name="storylet">The storylet to save.</param>
    void Save(Storylet storylet);

    /// <summary>
    /// Removes a storylet by ID.
    /// </summary>
    /// <param name="id">The storylet ID to remove.</param>
    /// <returns>True if the storylet was removed; false if it didn't exist.</returns>
    bool Delete(string id);

    /// <summary>
    /// Reloads all storylets from the data source.
    /// Useful for refreshing the repository during development.
    /// </summary>
    void Reload();
}
