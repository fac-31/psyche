# Storylets System - Implementation Plan

> [!NOTE]
> This document provides detailed technical specifications for implementing the quality-based narrative system (storylets) in Psyche.

## Table of Contents
1. [System Overview](#system-overview)
2. [Architecture](#architecture)
3. [Implementation Phases](#implementation-phases)
4. [Class Structure](#class-structure)
5. [Integration Points](#integration-points)
6. [Design Decisions](#design-decisions)
7. [Success Criteria](#success-criteria)

---

## System Overview

### What are Storylets?

Storylets are a quality-based narrative system where story content availability is determined by character stats/qualities. Based on Emily Short's quality-based narrative design, storylets consist of three components:

1. **Content**: Narrative text, dialogue, descriptions
2. **Prerequisites**: Conditions that determine when content is available (quality requirements)
3. **Effects**: Changes to world/character state after storylet execution

### Key Concepts

- **Qualities**: Numerical values that track narrative state (separate from battle stats)
  - **Progress Stats**: Track advancement through story arcs
  - **Menace Stats**: Create obstacles requiring management
  - **Resource Stats**: Enable/disable actions based on availability
  - **Metric Stats**: Abstract measurements for optional content

- **Emergent Narrative**: Multiple storylines can influence each other through shared stat pools
- **Dynamic Availability**: Only contextually relevant storylets appear based on current qualities
- **Non-Linear Structure**: Storylets enable branching and recombination without traditional tree structures

### References
- [Emily Short's Quality-Based Narrative](https://emshort.blog/category/quality-based-narrative/)
- Project roadmap: `docs/dev/roadmap/storylets-MVP.md`

---

## Architecture

### High-Level Components

```
┌─────────────────────────────────────────────────────┐
│                  Game Loop                          │
│              (Program.cs / GameManager)             │
└────────────────────┬────────────────────────────────┘
                     │
          ┌──────────┴──────────┐
          ▼                     ▼
    ┌──────────┐         ┌──────────┐
    │ Battle   │         │Narrative │
    │ System   │────────▶│ Manager  │
    └──────────┘         └─────┬────┘
          │                    │
          │              ┌─────┴─────┐
          │              ▼           ▼
          │      ┌─────────────┐ ┌────────────┐
          │      │  Storylet   │ │  Quality   │
          │      │ Repository  │ │  Manager   │
          │      └──────┬──────┘ └─────┬──────┘
          │             │               │
          └─────────────┼───────────────┘
                        ▼
                  ┌──────────┐
                  │Character │
                  │  Model   │
                  └──────────┘
```

### Core Systems

1. **Quality System**
   - `Quality` model: Represents narrative stats
   - `QualityManager`: Tracks and modifies qualities
   - Integrated into `Character` model

2. **Storylet Engine**
   - `Storylet` model: Content with prerequisites/effects
   - `IPrerequisite` interface: Requirement evaluation
   - `IEffect` interface: State modification

3. **Narrative Manager**
   - `StoryletRepository`: Storage and retrieval
   - `StoryletAvailability`: Filters available content
   - `NarrativeManager`: Main API orchestration

4. **UI Layer**
   - `StoryletUI`: Console-based display
   - Integration with existing menu systems

---

## Implementation Phases

### Phase 1: Core Quality System

**Objective**: Establish quality/stat tracking independent of battle stats

#### Tasks
1. Create `Models/Quality.cs`
   ```csharp
   public enum QualityType { Progress, Menace, Resource, Metric }

   public class Quality
   {
       public string Id { get; set; }
       public string Name { get; set; }
       public string Description { get; set; }
       public QualityType Type { get; set; }
       public int Value { get; set; }
       public int MinValue { get; set; }
       public int MaxValue { get; set; }
   }
   ```

2. Create `Models/QualityCollection.cs`
   ```csharp
   public class QualityCollection
   {
       private Dictionary<string, Quality> _qualities;
       public void AddQuality(Quality quality);
       public void ModifyQuality(string id, int delta);
       public int GetQualityValue(string id);
       public bool HasQuality(string id);
   }
   ```

3. Extend `Models/Character.cs`
   - Add `QualityCollection Qualities { get; set; }`
   - Ensure serialization compatibility

4. Create `Systems/QualityManager.cs`
   ```csharp
   public class QualityManager
   {
       public void RegisterQuality(Quality quality);
       public void ModifyCharacterQuality(Character character, string qualityId, int delta);
       public event EventHandler<QualityChangedEventArgs> QualityChanged;
   }
   ```

#### Deliverables
- [ ] Quality model with type system
- [ ] QualityCollection for managing character qualities
- [ ] Character integration
- [ ] QualityManager with event notifications
- [ ] Unit tests for quality modifications

---

### Phase 2: Storylet Core Engine

**Objective**: Create storylet models with prerequisite and effect systems

#### Tasks

1. Create `Models/Storylet.cs`
   ```csharp
   public class Storylet
   {
       public string Id { get; set; }
       public string Title { get; set; }
       public string Description { get; set; }
       public string Content { get; set; }
       public List<IPrerequisite> Prerequisites { get; set; }
       public List<IEffect> Effects { get; set; }
       public int Priority { get; set; }
       public string Category { get; set; }
       public List<string> Tags { get; set; }
   }
   ```

2. Create prerequisite system in `Systems/Prerequisites/`
   ```csharp
   public interface IPrerequisite
   {
       bool IsMet(Character character);
       string GetDisplayText();
   }

   public class QualityRequirement : IPrerequisite
   {
       public string QualityId { get; set; }
       public int? MinValue { get; set; }
       public int? MaxValue { get; set; }
   }

   public class CompoundPrerequisite : IPrerequisite
   {
       public enum LogicType { And, Or }
       public LogicType Logic { get; set; }
       public List<IPrerequisite> Prerequisites { get; set; }
   }

   public class StoryletPlayedRequirement : IPrerequisite
   {
       public string StoryletId { get; set; }
       public bool MustHavePlayed { get; set; }
   }
   ```

3. Create effect system in `Systems/Effects/`
   ```csharp
   public interface IEffect
   {
       void Apply(Character character);
       string GetDisplayText();
   }

   public class QualityEffect : IEffect
   {
       public string QualityId { get; set; }
       public int Delta { get; set; }
   }

   public class UnlockStoryletEffect : IEffect
   {
       public string StoryletId { get; set; }
   }

   public class CompoundEffect : IEffect
   {
       public List<IEffect> Effects { get; set; }
   }
   ```

#### Deliverables
- [ ] Storylet model
- [ ] Prerequisite interface and implementations
- [ ] Effect interface and implementations
- [ ] Unit tests for prerequisite evaluation
- [ ] Unit tests for effect application

---

### Phase 3: Storylet Management

**Objective**: Create repository, availability filtering, and orchestration

#### Tasks

1. Create `Data/StoryletRepository.cs`
   ```csharp
   public class StoryletRepository
   {
       public void LoadFromJson(string path);
       public Storylet GetById(string id);
       public IEnumerable<Storylet> GetByCategory(string category);
       public IEnumerable<Storylet> GetByTag(string tag);
       public IEnumerable<Storylet> GetAll();
   }
   ```

2. Create storylet JSON schema in `Data/Storylets/`
   ```json
   {
     "id": "storylet_001",
     "title": "A Chance Encounter",
     "description": "You notice someone watching you from the shadows.",
     "content": "A cloaked figure approaches...",
     "prerequisites": [
       { "type": "quality", "qualityId": "reputation", "minValue": 5 }
     ],
     "effects": [
       { "type": "quality", "qualityId": "intrigue", "delta": 1 }
     ],
     "priority": 10,
     "category": "exploration",
     "tags": ["urban", "mystery"]
   }
   ```

3. Create `Systems/StoryletAvailability.cs`
   ```csharp
   public class StoryletAvailability
   {
       public IEnumerable<Storylet> GetAvailableStorylets(
           Character character,
           IEnumerable<Storylet> candidates);
       public bool IsAvailable(Character character, Storylet storylet);
       public IEnumerable<Storylet> SortByPriority(IEnumerable<Storylet> storylets);
   }
   ```

4. Create `Systems/NarrativeManager.cs`
   ```csharp
   public class NarrativeManager
   {
       private StoryletRepository _repository;
       private StoryletAvailability _availability;
       private QualityManager _qualityManager;

       public IEnumerable<Storylet> GetAvailableStorylets(Character character, string category = null);
       public void ExecuteStorylet(Character character, Storylet storylet);
       public void MarkStoryletPlayed(Character character, string storyletId);
   }
   ```

#### Deliverables
- [ ] StoryletRepository with JSON loading
- [ ] JSON schema and sample data files
- [ ] StoryletAvailability calculator
- [ ] NarrativeManager orchestration
- [ ] Integration tests for full flow

---

### Phase 4: Integration & UI

**Objective**: Create console interface and integrate with game loop

#### Tasks

1. Create `UI/StoryletUI.cs`
   ```csharp
   public class StoryletUI
   {
       public void DisplayAvailableStorylets(IEnumerable<Storylet> storylets);
       public Storylet PromptStoryletSelection(IEnumerable<Storylet> storylets);
       public void DisplayStoryletContent(Storylet storylet);
       public void DisplayQualityChanges(IEnumerable<IEffect> effects);
   }
   ```

2. Update `Program.cs` or create `GameManager.cs`
   - Add narrative phase to game loop
   - Trigger storylets after battles
   - Allow exploration mode for storylet discovery

3. Create `Systems/BattleNarrativeIntegration.cs`
   - Define battle outcome → quality mappings
   - Victory grants progress qualities
   - Defeat might increase menace qualities

4. Update Character display to show qualities
   - Add quality summary to character stats
   - Show recent quality changes

#### Deliverables
- [ ] Console UI for storylets
- [ ] Game loop integration
- [ ] Battle-to-narrative bridge
- [ ] Character UI updates
- [ ] End-to-end manual testing

---

### Phase 5: Content & Testing

**Objective**: Create sample content and comprehensive test coverage

#### Tasks

1. Create sample storylet content in `Data/Storylets/demo/`
   - **Linear Arc**: 5 storylets showing progression
   - **Branching Paths**: 3 storylets with choice consequences
   - **Resource Management**: 2 storylets requiring quality spending
   - **Menace System**: 3 storylets showing escalating danger

2. Create comprehensive unit tests
   - Quality system (add, modify, bounds checking)
   - Prerequisite evaluation (all types)
   - Effect application (all types)
   - Storylet availability filtering

3. Create integration tests
   - Load storylets from JSON
   - Execute storylet and verify effects
   - Battle outcome → quality change → storylet unlock
   - Full game loop with narrative

4. Create example playthrough documentation
   - Document sample storylet flow
   - Show quality changes over time
   - Demonstrate emergent narrative

#### Deliverables
- [ ] 10-15 demo storylets
- [ ] Unit test suite (80%+ coverage)
- [ ] Integration test suite
- [ ] Example playthrough documentation

---

## Class Structure

### Directory Layout

```
psyche/
├── Models/
│   ├── Character.cs (updated)
│   ├── Quality.cs
│   ├── QualityCollection.cs
│   └── Storylet.cs
├── Systems/
│   ├── QualityManager.cs
│   ├── NarrativeManager.cs
│   ├── StoryletAvailability.cs
│   ├── BattleNarrativeIntegration.cs
│   ├── Prerequisites/
│   │   ├── IPrerequisite.cs
│   │   ├── QualityRequirement.cs
│   │   ├── CompoundPrerequisite.cs
│   │   └── StoryletPlayedRequirement.cs
│   └── Effects/
│       ├── IEffect.cs
│       ├── QualityEffect.cs
│       ├── UnlockStoryletEffect.cs
│       └── CompoundEffect.cs
├── Data/
│   ├── StoryletRepository.cs
│   └── Storylets/
│       ├── schema.json
│       └── demo/
│           ├── arc_beginning.json
│           ├── arc_middle.json
│           └── arc_end.json
├── UI/
│   └── StoryletUI.cs
└── Tests/
    ├── QualitySystemTests.cs
    ├── PrerequisiteTests.cs
    ├── EffectTests.cs
    ├── StoryletAvailabilityTests.cs
    └── NarrativeIntegrationTests.cs
```

### Key Interfaces

```csharp
// Core interfaces
public interface IPrerequisite
{
    bool IsMet(Character character);
    string GetDisplayText();
}

public interface IEffect
{
    void Apply(Character character);
    string GetDisplayText();
}

// Manager interfaces
public interface INarrativeManager
{
    IEnumerable<Storylet> GetAvailableStorylets(Character character, string category = null);
    void ExecuteStorylet(Character character, Storylet storylet);
}

public interface IQualityManager
{
    void ModifyQuality(Character character, string qualityId, int delta);
    int GetQualityValue(Character character, string qualityId);
}
```

---

## Integration Points

### 1. Character System Integration

**File**: `Models/Character.cs`

**Changes**:
- Add `QualityCollection Qualities` property
- Update serialization to include qualities
- Add helper methods for quality access

**Impact**: Minimal - additive only, no breaking changes

### 2. Battle System Integration

**File**: `Systems/BattleNarrativeIntegration.cs` (new)

**Purpose**: Bridge battle outcomes to narrative system

**Mappings**:
- Victory → `reputation` quality +1
- Enemy defeated → `combat_experience` quality +1
- Damage taken → `exhaustion` menace quality increase
- Items used → `resourcefulness` quality change

### 3. Game Loop Integration

**File**: `Program.cs` or `GameManager.cs`

**Changes**:
- Add narrative phase after battles
- Provide exploration mode for storylet discovery
- Save/load qualities with character data

**Flow**:
```
Start → Battle → Victory → Update Qualities → Show Available Storylets
  → Player Chooses Storylet → Apply Effects → Next Battle → ...
```

### 4. Serialization

**Requirements**:
- Qualities must serialize with character saves
- Played storylet history must persist
- JSON configuration for storylet content

---

## Design Decisions

### 1. Qualities vs Battle Stats

**Decision**: Keep qualities separate from battle stats (HP, Strength, etc.)

**Rationale**:
- Battle stats are mechanical (affect combat calculations)
- Qualities are narrative (affect story availability)
- Separation allows independent balance of combat and story
- Character can have high battle strength but low narrative reputation

**Implementation**: Qualities stored in separate `QualityCollection` on Character

### 2. Storylet Storage Format

**Decision**: Use JSON files for storylet content

**Rationale**:
- Easy content authoring without recompilation
- Non-programmers can create storylets
- Version control friendly
- Supports modding/extension

**Alternative Considered**: Hard-coded in C# - rejected due to inflexibility

### 3. Prerequisite Evaluation

**Decision**: Interface-based system with composable prerequisites

**Rationale**:
- Extensible - new prerequisite types without modifying core
- Composable - AND/OR logic via CompoundPrerequisite
- Testable - each type in isolation

**Example**:
```csharp
// Complex prerequisite: (Reputation >= 10 AND Intrigue >= 5) OR HasPlayedStorylet("intro")
new CompoundPrerequisite {
    Logic = Or,
    Prerequisites = [
        new CompoundPrerequisite {
            Logic = And,
            Prerequisites = [
                new QualityRequirement { QualityId = "reputation", MinValue = 10 },
                new QualityRequirement { QualityId = "intrigue", MinValue = 5 }
            ]
        },
        new StoryletPlayedRequirement { StoryletId = "intro", MustHavePlayed = true }
    ]
}
```

### 4. Effect System

**Decision**: Interface-based with immediate application

**Rationale**:
- Consistent with prerequisite design
- Effects apply immediately upon storylet execution
- Clear cause-and-effect for player

**Alternative Considered**: Delayed/queued effects - deferred for post-MVP

### 5. Storylet Discovery

**Decision**: Automatically filter and present available storylets

**Rationale**:
- Matches quality-based narrative philosophy
- Players only see contextually relevant options
- Reduces decision paralysis
- Creates sense of emergent story

**Implementation**: StoryletAvailability calculator filters based on prerequisites

### 6. Priority System

**Decision**: Storylets have priority values for display order

**Rationale**:
- Authors can emphasize critical story beats
- Important storylets appear first in lists
- Allows manual curation of discovery

**Default**: Priority 10 (normal), range 0-100

---

## Success Criteria

### MVP Complete When:

1. **Quality System Works**
   - ✅ Qualities can be added to characters
   - ✅ Quality values can be modified
   - ✅ Quality changes persist with character saves
   - ✅ Qualities are independent of battle stats

2. **Storylet Engine Functions**
   - ✅ Storylets can be defined with prerequisites and effects
   - ✅ Prerequisites correctly evaluate against character state
   - ✅ Effects properly modify character qualities
   - ✅ Compound prerequisites (AND/OR) work correctly

3. **Availability Filtering Works**
   - ✅ Only storylets matching prerequisites are shown
   - ✅ Storylets are sorted by priority
   - ✅ Played storylets can gate future content
   - ✅ Multiple storylets can be available simultaneously

4. **Integration Complete**
   - ✅ Battle outcomes affect qualities
   - ✅ Game loop includes narrative phase
   - ✅ Console UI displays storylets and quality changes
   - ✅ Player can choose and execute storylets

5. **Content Demonstrates System**
   - ✅ 10+ sample storylets created
   - ✅ Demonstrates linear progression
   - ✅ Demonstrates branching choices
   - ✅ Demonstrates resource management
   - ✅ Demonstrates menace/consequence mechanics

6. **Testing Coverage**
   - ✅ Unit tests for quality system (80%+ coverage)
   - ✅ Unit tests for prerequisites and effects
   - ✅ Integration tests for full narrative flow
   - ✅ Manual playthrough testing completed

### Performance Targets

- Storylet availability calculation: < 10ms for 100 storylets
- JSON loading: < 100ms for initial repository load
- Effect application: < 5ms per storylet execution

### Quality Targets

- No crashes or exceptions during normal gameplay
- Clear error messages for invalid storylet configurations
- Intuitive UI that guides player understanding
- Quality changes are always visible to player

---

## Next Steps After MVP

See `docs/dev/roadmap/storylets-MVP.md` section "3. Beyond MVP: Future Features" for post-MVP enhancements including:

- Advanced prerequisite types (time-based, random chance)
- Branching storylet chains
- Storylet decay and expiration
- Visual enhancements
- Storylet editor tooling
- Character relationship qualities
- Faction/reputation systems
- Story arc tracking and visualization

---

## References

- [Emily Short - Quality-Based Narrative](https://emshort.blog/category/quality-based-narrative/)
- [Failbetter Games - Fallen London](https://www.failbettergames.com/) (Reference implementation)
- Project Roadmap: `docs/dev/roadmap/storylets-MVP.md`
- Character System: `docs/dev/roadmap/character-mvp.md`
- Battle System: `docs/dev/roadmap/battleSystem-mvp.md`

---

**Last Updated**: 2025-11-17
**Status**: Planning Complete, Ready for Implementation
