namespace Psyche.Data.Json;

using System.Text.Json;
using Psyche.Models;
using Psyche.Systems.Prerequisites;
using Psyche.Systems.Effects;

/// <summary>
/// Converts between StoryletDto JSON representation and Storylet domain model.
/// Handles polymorphic deserialization of prerequisites and effects.
/// Supports JSONC (JSON with Comments) format.
/// </summary>
public static class StoryletJsonConverter
{
    /// <summary>
    /// JSON serialization options for JSONC support.
    /// </summary>
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    /// <summary>
    /// Converts a StoryletDto to a Storylet domain model.
    /// </summary>
    public static Storylet FromDto(StoryletDto dto)
    {
        var storylet = new Storylet
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            Content = dto.Content,
            Priority = dto.Priority,
            Category = dto.Category,
            Tags = new List<string>(dto.Tags)
        };

        // Convert prerequisites
        foreach (var prereqDto in dto.Prerequisites)
        {
            storylet.Prerequisites.Add(DeserializePrerequisite(prereqDto));
        }

        // Convert effects
        foreach (var effectDto in dto.Effects)
        {
            storylet.Effects.Add(DeserializeEffect(effectDto));
        }

        // Convert options
        foreach (var optionDto in dto.Options)
        {
            var option = new StoryletOption
            {
                Id = optionDto.Id,
                Text = optionDto.Text,
                Description = optionDto.Description,
                ResultText = optionDto.ResultText,
                Priority = optionDto.Priority,
                Tags = new List<string>(optionDto.Tags)
            };

            // Convert option prerequisites
            foreach (var prereqDto in optionDto.Prerequisites)
            {
                option.Prerequisites.Add(DeserializePrerequisite(prereqDto));
            }

            // Convert option effects
            foreach (var effectDto in optionDto.Effects)
            {
                option.Effects.Add(DeserializeEffect(effectDto));
            }

            storylet.Options.Add(option);
        }

        return storylet;
    }

    /// <summary>
    /// Converts a Storylet domain model to a StoryletDto.
    /// </summary>
    public static StoryletDto ToDto(Storylet storylet)
    {
        var dto = new StoryletDto
        {
            Id = storylet.Id,
            Title = storylet.Title,
            Description = storylet.Description,
            Content = storylet.Content,
            Priority = storylet.Priority,
            Category = storylet.Category,
            Tags = new List<string>(storylet.Tags)
        };

        // Convert prerequisites
        foreach (var prereq in storylet.Prerequisites)
        {
            dto.Prerequisites.Add(SerializePrerequisite(prereq));
        }

        // Convert effects
        foreach (var effect in storylet.Effects)
        {
            dto.Effects.Add(SerializeEffect(effect));
        }

        // Convert options
        foreach (var option in storylet.Options)
        {
            var optionDto = new StoryletOptionDto
            {
                Id = option.Id,
                Text = option.Text,
                Description = option.Description,
                ResultText = option.ResultText,
                Priority = option.Priority,
                Tags = new List<string>(option.Tags)
            };

            // Convert option prerequisites
            foreach (var prereq in option.Prerequisites)
            {
                optionDto.Prerequisites.Add(SerializePrerequisite(prereq));
            }

            // Convert option effects
            foreach (var effect in option.Effects)
            {
                optionDto.Effects.Add(SerializeEffect(effect));
            }

            dto.Options.Add(optionDto);
        }

        return dto;
    }

    /// <summary>
    /// Deserializes a prerequisite from DTO based on type discriminator.
    /// </summary>
    private static IPrerequisite DeserializePrerequisite(PrerequisiteDto dto)
    {
        return dto.Type switch
        {
            "AttributeRequirement" => DeserializeAttributeRequirement(dto),
            "QualityRequirement" => DeserializeQualityRequirement(dto),
            "StoryletPlayedRequirement" => DeserializeStoryletPlayedRequirement(dto),
            "CompoundPrerequisite" => DeserializeCompoundPrerequisite(dto),
            _ => throw new InvalidOperationException($"Unknown prerequisite type: {dto.Type}")
        };
    }

    /// <summary>
    /// Deserializes an effect from DTO based on type discriminator.
    /// </summary>
    private static IEffect DeserializeEffect(EffectDto dto)
    {
        return dto.Type switch
        {
            "AttributeEffect" => DeserializeAttributeEffect(dto),
            "QualityEffect" => DeserializeQualityEffect(dto),
            "UnlockStoryletEffect" => DeserializeUnlockStoryletEffect(dto),
            "CompoundEffect" => DeserializeCompoundEffect(dto),
            _ => throw new InvalidOperationException($"Unknown effect type: {dto.Type}")
        };
    }

    private static AttributeRequirement DeserializeAttributeRequirement(PrerequisiteDto dto)
    {
        var element = JsonSerializer.SerializeToElement(dto.Properties);
        return new AttributeRequirement
        {
            AttributeName = element.GetProperty("attributeName").GetString() ?? string.Empty,
            MinValue = element.TryGetProperty("minValue", out var minProp) ? minProp.GetInt32() : null,
            MaxValue = element.TryGetProperty("maxValue", out var maxProp) ? maxProp.GetInt32() : null
        };
    }

    private static QualityRequirement DeserializeQualityRequirement(PrerequisiteDto dto)
    {
        var element = JsonSerializer.SerializeToElement(dto.Properties);
        return new QualityRequirement
        {
            QualityId = element.GetProperty("qualityId").GetString() ?? string.Empty,
            MinValue = element.TryGetProperty("minValue", out var minProp) ? minProp.GetInt32() : null,
            MaxValue = element.TryGetProperty("maxValue", out var maxProp) ? maxProp.GetInt32() : null
        };
    }

    private static StoryletPlayedRequirement DeserializeStoryletPlayedRequirement(PrerequisiteDto dto)
    {
        var element = JsonSerializer.SerializeToElement(dto.Properties);
        return new StoryletPlayedRequirement
        {
            StoryletId = element.GetProperty("storyletId").GetString() ?? string.Empty,
            MustHavePlayed = element.GetProperty("mustHavePlayed").GetBoolean()
        };
    }

    private static CompoundPrerequisite DeserializeCompoundPrerequisite(PrerequisiteDto dto)
    {
        var element = JsonSerializer.SerializeToElement(dto.Properties);
        var compound = new CompoundPrerequisite();

        // Parse logic type
        if (element.TryGetProperty("logic", out var logicProp))
        {
            var logicStr = logicProp.GetString();
            compound.Logic = logicStr?.ToLower() == "or"
                ? CompoundPrerequisite.LogicType.Or
                : CompoundPrerequisite.LogicType.And;
        }

        // Parse nested prerequisites
        if (element.TryGetProperty("prerequisites", out var prereqsProp))
        {
            var prereqDtos = JsonSerializer.Deserialize<List<PrerequisiteDto>>(prereqsProp.GetRawText(), JsonOptions);
            if (prereqDtos != null)
            {
                foreach (var prereqDto in prereqDtos)
                {
                    compound.Prerequisites.Add(DeserializePrerequisite(prereqDto));
                }
            }
        }

        return compound;
    }

    private static AttributeEffect DeserializeAttributeEffect(EffectDto dto)
    {
        var element = JsonSerializer.SerializeToElement(dto.Properties);
        return new AttributeEffect
        {
            AttributeName = element.GetProperty("attributeName").GetString() ?? string.Empty,
            Delta = element.GetProperty("delta").GetInt32()
        };
    }

    private static QualityEffect DeserializeQualityEffect(EffectDto dto)
    {
        var element = JsonSerializer.SerializeToElement(dto.Properties);
        return new QualityEffect
        {
            QualityId = element.GetProperty("qualityId").GetString() ?? string.Empty,
            Delta = element.GetProperty("delta").GetInt32()
        };
    }

    private static UnlockStoryletEffect DeserializeUnlockStoryletEffect(EffectDto dto)
    {
        var element = JsonSerializer.SerializeToElement(dto.Properties);
        return new UnlockStoryletEffect
        {
            StoryletId = element.GetProperty("storyletId").GetString() ?? string.Empty
        };
    }

    private static CompoundEffect DeserializeCompoundEffect(EffectDto dto)
    {
        var element = JsonSerializer.SerializeToElement(dto.Properties);
        var compound = new CompoundEffect();

        // Parse nested effects
        if (element.TryGetProperty("effects", out var effectsProp))
        {
            var effectDtos = JsonSerializer.Deserialize<List<EffectDto>>(effectsProp.GetRawText(), JsonOptions);
            if (effectDtos != null)
            {
                foreach (var effectDto in effectDtos)
                {
                    compound.Effects.Add(DeserializeEffect(effectDto));
                }
            }
        }

        return compound;
    }

    // Serialization methods (for saving storylets back to JSON)

    private static PrerequisiteDto SerializePrerequisite(IPrerequisite prerequisite)
    {
        return prerequisite switch
        {
            AttributeRequirement ar => new PrerequisiteDto
            {
                Type = "AttributeRequirement",
                Properties = new Dictionary<string, object>
                {
                    ["attributeName"] = ar.AttributeName,
                    ["minValue"] = ar.MinValue ?? 0,
                    ["maxValue"] = ar.MaxValue ?? 100
                }
            },
            QualityRequirement qr => new PrerequisiteDto
            {
                Type = "QualityRequirement",
                Properties = new Dictionary<string, object>
                {
                    ["qualityId"] = qr.QualityId,
                    ["minValue"] = qr.MinValue ?? int.MinValue,
                    ["maxValue"] = qr.MaxValue ?? int.MaxValue
                }
            },
            StoryletPlayedRequirement spr => new PrerequisiteDto
            {
                Type = "StoryletPlayedRequirement",
                Properties = new Dictionary<string, object>
                {
                    ["storyletId"] = spr.StoryletId,
                    ["mustHavePlayed"] = spr.MustHavePlayed
                }
            },
            CompoundPrerequisite cp => new PrerequisiteDto
            {
                Type = "CompoundPrerequisite",
                Properties = new Dictionary<string, object>
                {
                    ["logic"] = cp.Logic.ToString(),
                    ["prerequisites"] = cp.Prerequisites.Select(SerializePrerequisite).ToList()
                }
            },
            _ => throw new InvalidOperationException($"Unknown prerequisite type: {prerequisite.GetType().Name}")
        };
    }

    private static EffectDto SerializeEffect(IEffect effect)
    {
        return effect switch
        {
            AttributeEffect ae => new EffectDto
            {
                Type = "AttributeEffect",
                Properties = new Dictionary<string, object>
                {
                    ["attributeName"] = ae.AttributeName,
                    ["delta"] = ae.Delta
                }
            },
            QualityEffect qe => new EffectDto
            {
                Type = "QualityEffect",
                Properties = new Dictionary<string, object>
                {
                    ["qualityId"] = qe.QualityId,
                    ["delta"] = qe.Delta
                }
            },
            UnlockStoryletEffect use => new EffectDto
            {
                Type = "UnlockStoryletEffect",
                Properties = new Dictionary<string, object>
                {
                    ["storyletId"] = use.StoryletId
                }
            },
            CompoundEffect ce => new EffectDto
            {
                Type = "CompoundEffect",
                Properties = new Dictionary<string, object>
                {
                    ["effects"] = ce.Effects.Select(SerializeEffect).ToList()
                }
            },
            _ => throw new InvalidOperationException($"Unknown effect type: {effect.GetType().Name}")
        };
    }
}
