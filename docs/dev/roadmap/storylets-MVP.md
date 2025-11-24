# Storylets MVP

> [!NOTE]
> Quality-based narrative system where story content availability is determined by character stats/qualities. Implements the approach found [here](https://emshort.blog/category/quality-based-narrative/)

---

## 1. Tasks
### 1a. Open Tasks
#### 1a1. Due Tasks
<!-- No due tasks -->

#### 1a2. Other Tasks
- Create remaining 8-13 demo storylets to complete M5 content requirements
  - Linear progression arc: 3 more storylets needed (2/5 complete)
  - Branching paths: 3 storylets needed
  - Resource management: 2 storylets needed
  - Menace/consequence system: 3 storylets needed

### 1b. Blocked Tasks
- **M4: Integration & UI** - Blocked until core quality system integration is verified
  - StoryletUI console interface
  - Game loop integration (narrative phase)
  - BattleNarrativeIntegration
  - Character UI updates
  - Battle → Narrative → Exploration flow
  - Save/load integration

---

## 2. MVP Milestones

### M1: Quality System

**Core Attribute System** (6 personality scales 0-100):
- Self-Assurance: inadequacy (0) ↔ self-assurance (50) ↔ arrogance (100)
- Compassion: unempathetic (0) ↔ compassionate (50) ↔ lack of boundaries (100)
- Ambition: unimaginative (0) ↔ ambitious (50) ↔ fantasist (100)
- Drive: passive (0) ↔ driven (50) ↔ steamroller (100)
- Discernment: gullible (0) ↔ discerning (50) ↔ overly critical (100)
- Bravery: selfish (0) ↔ brave (50) ↔ reckless (100)

**Storylet Quality System**:
- Quality model definition (Progress, Menace, Resource, Metric types)
- QualityCollection for managing character qualities
- Character integration (add 6 core attributes + Qualities property + Drive property)
- QualityManager for tracking and modification
- Quality change event notifications
- Serialization support for core attributes, drive, and qualities

**Defined Qualities for MVP**:
- Progress: `main_story_progress`, `character_insight`, `faction_standing`
- Menace: `psychological_strain`, `enemies_made`
- Resource: `social_capital`, `secrets_learned`
- Metric: `battles_experienced`, `key_choices_made`

### M2: Storylet Core Engine
- Storylet model definition (content, prerequisites, effects)
- IPrerequisite interface and implementations
  - AttributeRequirement (check core attributes like Bravery ≥ 60)
  - QualityRequirement (check storylet qualities like social_capital ≥ 10)
  - CompoundPrerequisite (AND/OR logic)
  - StoryletPlayedRequirement (for sequencing)
- IEffect interface and implementations
  - AttributeEffect (modify core attributes: Bravery +5)
  - QualityEffect (modify storylet qualities: psychological_strain -10)
  - UnlockStoryletEffect (enable new storylets)
  - CompoundEffect (multiple effects)
- Prerequisite evaluation logic
- Effect application system

**Example Prerequisites**:
- Simple: Bravery ≥ 60
- Simple: social_capital ≥ 10
- Compound: (Compassion ≥ 60 AND psychological_strain < 50)
- Complex: ((Bravery ≥ 60 OR Discernment ≥ 70) AND social_capital ≥ 5)

**Example Effects**:
- Modify attribute: Bravery +5, Self-Assurance +2
- Modify quality: psychological_strain -10, social_capital +5
- Mixed: Compassion +3, enemies_made -1, main_story_progress +1

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
  - **Linear progression arc** (5 storylets): Uses `main_story_progress` to unlock sequentially. Gates on core attributes (Bravery, Compassion) and increases `character_insight`
  - **Branching paths** (3 storylets): Choices affect both core attributes and qualities. High Compassion path vs High Bravery path, different `social_capital` and `enemies_made` outcomes
  - **Resource management** (2 storylets): Requires spending `social_capital` or `secrets_learned`, rewards with attribute boosts
  - **Menace/consequence system** (3 storylets): Tracks `psychological_strain` buildup. High strain unlocks crisis storylets. Resolution decreases strain, increases `character_insight`
- Unit test suite for quality system
  - Test core attribute modifications
  - Test storylet quality modifications
  - Test bounds checking (0-100 for attributes)
- Unit tests for prerequisites and effects
  - AttributeRequirement and QualityRequirement
  - Compound prerequisites with attributes and qualities
  - AttributeEffect and QualityEffect
- Unit tests for storylet availability
- Integration tests (full narrative flow)
  - Battle → attribute changes → storylet unlock
  - Storylet effect → new storylets available
- End-to-end playthrough testing
- Example playthrough documentation showing attribute and quality changes

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

#### ✅ M2: Storylet Core Engine (Completed 2025-11-24)
- ✅ Storylet model definition with content, prerequisites, effects, and options
- ✅ StoryletOption model for choices within storylets
- ✅ StoryletValidationResult for validation feedback
- ✅ IPrerequisite interface and implementations:
  - AttributeRequirement (check core attributes like Bravery ≥ 60)
  - QualityRequirement (check storylet qualities like social_capital ≥ 10)
  - CompoundPrerequisite (AND/OR logic)
  - StoryletPlayedRequirement (for sequencing)
- ✅ IEffect interface and implementations:
  - AttributeEffect (modify core attributes: Bravery +5)
  - QualityEffect (modify storylet qualities: psychological_strain -10)
  - UnlockStoryletEffect (enable new storylets)
  - CompoundEffect (multiple effects)
- ✅ Prerequisite evaluation logic with validation
- ✅ Effect application system

#### ✅ M3: Storylet Management (Completed 2025-11-24)
- ✅ IStoryletRepository interface (GetById, GetAll, GetByCategory, GetByTags, Save, Delete, Reload)
- ✅ JsonStoryletRepository implementation with file-based storage
- ✅ JSON storylet schema definition (StoryletDto, StoryletOptionDto, PrerequisiteDto, EffectDto)
- ✅ JSON loading and parsing with JSONC support (comments and trailing commas)
- ✅ StoryletJsonConverter for bidirectional serialization with polymorphic type handling
- ✅ StoryletAvailability calculator (IsAvailable, GetAvailableOptions methods)
- ✅ Priority/weighting system for display order (priority property on storylets and options)
- ✅ Storylet validation system (Validate method with error collection)
- ✅ Played storylet tracking support via StoryletPlayedRequirement

### 4b. Completed Tasks
#### 4b1. Record of Past Deadlines
<!-- No past deadlines -->

#### 4b2. Record of Other Completed Tasks

**M5: Content & Testing (Partial - 2025-11-24)**
- ✅ Created 2/15 demo storylets:
  - `first_encounter.jsonc` - Simple introductory storylet with attribute-based choices
  - `dangerous_choice.jsonc` - Complex storylet with compound prerequisites and branching paths
- ✅ Comprehensive unit test suite (1,675+ lines):
  - JsonStoryletRepositoryTests (337 lines) - Repository CRUD, file loading, validation
  - StoryletJsonConverterTests (532 lines) - Bidirectional conversion, polymorphic deserialization
  - StoryletOptionTests (221 lines) - Option evaluation and effect application
  - StoryletOptionsAvailabilityTests (350 lines) - Complex availability logic
  - StoryletValidationTests (235 lines) - Validation rules
- ✅ Integration tests (509 lines):
  - StoryletChoiceIntegrationTests - End-to-end storylet execution with state changes
- ✅ Interactive walkthrough demo:
  - StoryletWalkthroughDemo.cs - Step-by-step demonstration of storylet flow
  - demo-storylet.sh - Shell script to run demo
  - Documentation in CLAUDE.md
