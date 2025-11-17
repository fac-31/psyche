namespace Psyche.Systems.Prerequisites;

using Psyche.Models.Mocks;

/// <summary>
/// Prerequisite that combines multiple prerequisites with AND/OR logic.
/// Example: (Bravery >= 60 OR Discernment >= 70) AND social_capital >= 5.
/// </summary>
public class CompoundPrerequisite : IPrerequisite
{
    /// <summary>Logic type for combining prerequisites.</summary>
    public enum LogicType
    {
        /// <summary>All prerequisites must be met (logical AND).</summary>
        And,
        /// <summary>At least one prerequisite must be met (logical OR).</summary>
        Or
    }

    /// <summary>The logic to use when combining prerequisites (default: AND).</summary>
    public LogicType Logic { get; set; } = LogicType.And;

    /// <summary>The list of prerequisites to combine.</summary>
    public List<IPrerequisite> Prerequisites { get; set; } = new();

    /// <summary>
    /// Evaluates whether the compound prerequisite is met.
    /// For AND: all prerequisites must be met.
    /// For OR: at least one prerequisite must be met.
    /// </summary>
    public bool IsMet(Character character)
    {
        if (Prerequisites.Count == 0)
            return true;

        return Logic switch
        {
            LogicType.And => Prerequisites.All(p => p.IsMet(character)),
            LogicType.Or => Prerequisites.Any(p => p.IsMet(character)),
            _ => throw new InvalidOperationException($"Unknown logic type: {Logic}")
        };
    }

    /// <summary>Gets a human-readable description of this compound requirement.</summary>
    public string GetDisplayText()
    {
        if (Prerequisites.Count == 0)
            return "No requirements";

        var separator = Logic == LogicType.And ? " AND " : " OR ";
        var parts = Prerequisites.Select(p => p.GetDisplayText());
        return $"({string.Join(separator, parts)})";
    }
}
