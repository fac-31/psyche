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

### The Stat System: Core Attributes vs Storylet Qualities

Psyche uses a unique stat system with two layers:

#### Core Attributes (The 6 Personality Scales)
The game has **NO traditional RPG stats** (HP, Strength, Defense, etc.). Instead, characters are defined by 6 personality attributes, each a scale from 0-100:

1. **Self-Assurance**: inadequacy (0) ↔ self-assurance (50) ↔ arrogance (100)
2. **Compassion**: unempathetic (0) ↔ compassionate (50) ↔ lack of boundaries (100)
3. **Ambition**: unimaginative (0) ↔ ambitious (50) ↔ fantasist (100)
4. **Drive**: passive (0) ↔ driven (50) ↔ steamroller (100)
5. **Discernment**: gullible (0) ↔ discerning (50) ↔ overly critical (100)
6. **Bravery**: selfish (0) ↔ brave (50) ↔ reckless (100)

These attributes:
- Affect **both** combat outcomes (mechanics TBD) and storylet availability
- Are the fundamental stats that define a character
- Can be prerequisites for storylets (e.g., "requires Bravery ≥ 60")
- Are modified by battle outcomes and storylet effects

#### Storylet Qualities (Additional Narrative Stats)
In addition to the 6 core attributes, the storylet system tracks additional qualities:

**Progress Qualities** (advancement through story):
- `main_story_progress`: Tracks narrative arc advancement
- `character_insight`: Understanding of own psychology
- `faction_standing`: Relationship with specific groups

**Menace Qualities** (obstacles and danger):
- `psychological_strain`: Mental/emotional stress accumulation
- `enemies_made`: Antagonistic attention

**Resource Qualities** (expendable/manageable stats):
- `social_capital`: Influence and connections
- `secrets_learned`: Information advantage

**Metric Qualities** (counters and trackers):
- `battles_experienced`: Combat encounter count
- `key_choices_made`: Major decision tracker

#### How They Work Together
- **Storylets can gate on both**: "Requires Compassion ≥ 60 AND social_capital ≥ 10"
- **Effects can modify both**: A storylet might increase Bravery by 5 AND decrease psychological_strain by 10
- **Emergent narrative**: Core attributes shape personality, qualities track narrative consequences
- **Example**: A character with high Bravery (70) but high psychological_strain (80) might see different storylets than one with moderate Bravery (50) and low strain (20)

#### Character Drives
At character creation, players choose a **Drive** (based on Enneagram types):
- Affects starting attribute values
- Defines win condition for the character's arc
- May be referenced as a quality in certain storylets
- Examples: "The Reformer" (wants to be good), "The Individualist" (seeks significance), "The Enthusiast" (seeks satisfaction)

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
          │             │              │
          └─────────────┼──────────────┘
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
- Ensure the 6 core attributes are defined as properties (Self-Assurance, Compassion, Ambition, Drive, Discernment, Bravery)
- Add `Drive DriveType` property for character creation choice
- Add `QualityCollection Qualities` property for storylet-specific qualities
- Update serialization to include all attributes, drive, and qualities
- Add helper methods for quality access
- Add methods to modify core attributes (for battle/storylet effects)

**Core Attributes vs Qualities**:
- Core attributes are stored directly as properties on Character
- Storylet qualities are stored in the QualityCollection
- Prerequisites can check either type using unified interface
- Effects can modify either type

**Impact**: Minimal - additive only, no breaking changes to existing Character structure

### 2. Battle System Integration

**File**: `Systems/BattleNarrativeIntegration.cs` (new)

**Purpose**: Bridge battle outcomes to narrative system by affecting both core attributes and storylet qualities

**Battle Outcome Mappings**:

**Core Attribute Changes**:
- Victory → Bravery +2, Self-Assurance +1
- Defeat (narrative consequence) → Self-Assurance -3, psychological_strain +15
- Reckless tactics → Bravery shifts toward 100 (reckless)
- Compassionate choices in battle → Compassion +2
- Strategic victory → Discernment +1

**Storylet Quality Changes**:
- Victory → `battles_experienced` +1
- Victory → `social_capital` +1 (reputation from success)
- Defeat → `psychological_strain` +10 (stress accumulation)
- Defeat → `enemies_made` +1 (antagonist attention)

**Notes**:
- Combat mechanics using the 6 attributes are TBD
- Battle system will determine how attributes affect combat (e.g., high Bravery might enable aggressive tactics)
- Defeat leads to narrative consequences (menace buildup, attribute shifts) not death

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

### 1. Core Attributes vs Storylet Qualities

**Decision**: The 6 personality attributes are the only base stats in the game (no HP, Strength, etc.). Additional storylet-specific qualities track narrative state separately.

**The 6 Core Attributes** (scales 0-100):
- **Self-Assurance**: inadequacy (0) ↔ self-assurance (50) ↔ arrogance (100)
- **Compassion**: unempathetic (0) ↔ compassionate (50) ↔ lack of boundaries (100)
- **Ambition**: unimaginative (0) ↔ ambitious (50) ↔ fantasist (100)
- **Drive**: passive (0) ↔ driven (50) ↔ steamroller (100)
- **Discernment**: gullible (0) ↔ discerning (50) ↔ overly critical (100)
- **Bravery**: selfish (0) ↔ brave (50) ↔ reckless (100)

**Rationale**:
- Core attributes affect BOTH combat (mechanics TBD) and storylet availability
- Storylet qualities are additional narrative stats (reputation, stress, story progress)
- This creates emergent gameplay where personality affects all aspects
- Storylets can gate on either core attributes OR additional qualities
- Example: A storylet might require Compassion ≥ 60 AND reputation ≥ 10

**Implementation**:
- Core attributes stored directly on `Character` model
- Storylet qualities stored in separate `QualityCollection` on Character

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

### 7. Character Drives System

**Decision**: Players choose a Drive at character creation based on Enneagram personality types

**Rationale**:
- Provides character motivation and psychological depth
- Defines win condition specific to each character arc
- Affects starting attribute distribution
- May be referenced in storylet prerequisites (e.g., "only available to The Reformer")
- Creates replay value through different character motivations

**Implementation**:
- Drive enum with values like: TheReformer, TheIndividualist, TheEnthusiast
- Stored on Character model
- Win condition evaluated based on Drive type (e.g., The Reformer wins by achieving high Compassion and Self-Assurance)
- Can be used as a quality identifier in prerequisites

**Example Drives**:
- **The Reformer (E1)**: Wants to be good → Win condition: Compassion ≥ 70, Discernment ≥ 70
- **The Individualist (E4)**: Seeks significance → Win condition: main_story_progress complete, character_insight ≥ 80
- **The Enthusiast (E7)**: Seeks satisfaction → Win condition: low psychological_strain, high social_capital

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
