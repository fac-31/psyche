namespace Psyche.Models.Mocks;

/// <summary>
/// MOCK CLASS - Temporary implementation until M1 (Quality System) is complete.
/// Represents the 6 core personality attributes (0-100 scales).
/// Replace with actual implementation from M1 when available.
/// </summary>
public class CoreAttributes
{
    private const int MinValue = 0;
    private const int MaxValue = 100;

    /// <summary>
    /// Self-Assurance: inadequacy (0) ↔ self-assurance (50) ↔ arrogance (100)
    /// </summary>
    public int SelfAssurance { get; set; } = 50;

    /// <summary>
    /// Compassion: unempathetic (0) ↔ compassionate (50) ↔ lack of boundaries (100)
    /// </summary>
    public int Compassion { get; set; } = 50;

    /// <summary>
    /// Ambition: unimaginative (0) ↔ ambitious (50) ↔ fantasist (100)
    /// </summary>
    public int Ambition { get; set; } = 50;

    /// <summary>
    /// Drive: passive (0) ↔ driven (50) ↔ steamroller (100)
    /// </summary>
    public int Drive { get; set; } = 50;

    /// <summary>
    /// Discernment: gullible (0) ↔ discerning (50) ↔ overly critical (100)
    /// </summary>
    public int Discernment { get; set; } = 50;

    /// <summary>
    /// Bravery: selfish (0) ↔ brave (50) ↔ reckless (100)
    /// </summary>
    public int Bravery { get; set; } = 50;

    /// <summary>
    /// Gets an attribute value by name (for dynamic access).
    /// </summary>
    /// <param name="attributeName">The name of the attribute to retrieve.</param>
    /// <returns>The current value of the specified attribute.</returns>
    /// <exception cref="ArgumentException">Thrown when the attribute name is not recognized.</exception>
    public int GetAttributeValue(string attributeName)
    {
        return attributeName switch
        {
            "SelfAssurance" => SelfAssurance,
            "Compassion" => Compassion,
            "Ambition" => Ambition,
            "Drive" => Drive,
            "Discernment" => Discernment,
            "Bravery" => Bravery,
            _ => throw new ArgumentException($"Unknown attribute: {attributeName}", nameof(attributeName))
        };
    }

    /// <summary>
    /// Modifies an attribute value, clamping to valid range (0-100).
    /// </summary>
    /// <param name="attributeName">The name of the attribute to modify.</param>
    /// <param name="delta">The amount to add (positive) or subtract (negative).</param>
    /// <exception cref="ArgumentException">Thrown when the attribute name is not recognized.</exception>
    public void ModifyAttribute(string attributeName, int delta)
    {
        var newValue = GetAttributeValue(attributeName) + delta;
        newValue = Math.Clamp(newValue, MinValue, MaxValue);

        switch (attributeName)
        {
            case "SelfAssurance":
                SelfAssurance = newValue;
                break;
            case "Compassion":
                Compassion = newValue;
                break;
            case "Ambition":
                Ambition = newValue;
                break;
            case "Drive":
                Drive = newValue;
                break;
            case "Discernment":
                Discernment = newValue;
                break;
            case "Bravery":
                Bravery = newValue;
                break;
            default:
                throw new ArgumentException($"Unknown attribute: {attributeName}", nameof(attributeName));
        }
    }
}
