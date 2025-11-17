# Storylets MVP

> [!NOTE]
> Quality-based narrative system where story content availability is determined by character stats/qualities. Implements the approach found [here](https://emshort.blog/category/quality-based-narrative/)

---

## 1. Tasks
### 1a. Open Tasks
#### 1a1. Due Tasks
<!-- No due tasks -->

#### 1a2. Other Tasks
<!-- No other tasks -->

### 1b. Blocked Tasks
<!-- No blocked tasks -->

---

## 2. MVP Milestones

### M1: Quality System
- Quality model definition (Progress, Menace, Resource, Metric types)
- QualityCollection for managing character qualities
- Character integration (add Qualities property)
- QualityManager for tracking and modification
- Quality change event notifications
- Serialization support for qualities

### M2: Storylet Core Engine
- Storylet model definition (content, prerequisites, effects)
- IPrerequisite interface and implementations
  - QualityRequirement (min/max value checks)
  - CompoundPrerequisite (AND/OR logic)
  - StoryletPlayedRequirement (for sequencing)
- IEffect interface and implementations
  - QualityEffect (modify quality values)
  - UnlockStoryletEffect (enable new storylets)
  - CompoundEffect (multiple effects)
- Prerequisite evaluation logic
- Effect application system

### M3: Storylet Management
- StoryletRepository (storage and retrieval)
- JSON storylet schema definition
- JSON loading and parsing
- StoryletAvailability calculator (filter by prerequisites)
- Priority/weighting system for display order
- NarrativeManager (main API orchestration)
- Storylet execution and effect application
- Played storylet tracking

### M4: Integration & UI
- StoryletUI console interface
  - Display available storylets
  - Show quality requirements
  - Present narrative content
  - Display quality changes
- Game loop integration (narrative phase)
- BattleNarrativeIntegration (battle outcomes → qualities)
- Character UI updates (show qualities)
- Battle → Narrative → Exploration flow
- Save/load integration for qualities and played history

### M5: Content & Testing
- Create 10-15 demo storylets
  - Linear progression arc (5 storylets)
  - Branching paths (3 storylets)
  - Resource management (2 storylets)
  - Menace/consequence system (3 storylets)
- Unit test suite for quality system
- Unit tests for prerequisites and effects
- Unit tests for storylet availability
- Integration tests (full narrative flow)
- End-to-end playthrough testing
- Example playthrough documentation

---

## 3. Beyond MVP: Future Features

### Advanced Prerequisites
- Time-based prerequisites (storylet available after X turns)
- Random chance prerequisites (percentage-based availability)
- Season/phase-based prerequisites
- Complex stat calculations
- Equipment-based prerequisites
- Party composition prerequisites

### Advanced Storylet Features
- Storylet chains and sequences
- Storylet decay and expiration (limited-time availability)
- Repeatable storylets with cooldowns
- Branching storylet trees
- Conditional content variations
- Multiple choice options within storylets
- Storylet consequences affecting multiple characters

### Narrative Quality of Life
- Story arc tracking and visualization
- Quest log integration
- Storylet history viewer
- Hint system for unlocking storylets
- Storylet preview (show locked storylets with requirements)
- Favorite/bookmark storylets
- Search and filter storylets

### Character Relationship System
- Character relationship qualities
- Relationship-based storylets
- Companion character integration
- Dialogue system tied to relationships
- Relationship status effects in battle

### Faction and Reputation
- Multiple faction systems
- Faction-specific storylets
- Reputation tracking per faction
- Faction conflicts and alliances
- Faction-based rewards and penalties

### Visual Enhancements
- Storylet images and artwork
- Character portraits in storylets
- Location backgrounds
- Animated transitions
- Visual quality change indicators
- Storylet category icons

### Content Creation Tools
- Storylet editor GUI
- Prerequisite builder interface
- Effect designer tool
- Storylet testing sandbox
- Content validation tools
- Storylet analytics (track player choices)

### Advanced Systems
- Procedurally generated storylets
- Storylet templates and variations
- Dynamic quality adjustment (difficulty scaling)
- Storylet recommendations based on play style
- Meta-narrative features
- New Game+ with storylet variations

---

## 4. Work Record
### 4a. Completed Milestones
<!-- No completed milestones -->

### 4b. Completed Tasks
#### 4b1. Record of Past Deadlines
<!-- No past deadlines -->

#### 4b2. Record of Other Completed Tasks
<!-- No other completed tasks -->
