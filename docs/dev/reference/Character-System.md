# Character System

## Types add/minus 20

1. Donald Trump [ + ambitious, - compassion ]
2. Battlefield Medic [ + brave, - discerning ]
3. CEO [ + xxx, - xxx ]

---

## Character Drives

### Overview
At character creation, players choose a **Drive** based on Enneagram personality types. The Drive serves three key purposes:
1. **Shapes starting attributes** - Each Drive modifies initial attribute values
2. **Defines win condition** - Each Drive has unique victory criteria for character arc completion
3. **Influences narrative** - Some storylets may be Drive-specific or reference the Drive

### The 9 Drives (Enneagram Types)

#### E1: The Reformer
**Core Motivation**: Want to be good, to have integrity, to be balanced

**Starting Attribute Modifiers**:
- Discernment +15 (more critical/judging)
- Compassion +10 (care about doing right)
- Self-Assurance -5 (self-critical)

**Win Condition**:
- Compassion ≥ 70 AND Discernment ≥ 70 AND psychological_strain < 30
- (Achieved goodness with wisdom and peace)

**Character Arc**: Overcoming perfectionism while maintaining principles

---

#### E2: The Helper
**Core Motivation**: Want to be loved, to be needed, to be appreciated

**Starting Attribute Modifiers**:
- Compassion +15 (highly empathetic)
- Drive +10 (motivated to help)
- Bravery -5 (avoid conflict that threatens relationships)

**Win Condition**:
- Compassion ≥ 60 AND Compassion < 80 AND social_capital ≥ 80 AND Self-Assurance ≥ 60
- (Healthy boundaries while maintaining connections)

**Character Arc**: Learning self-care and healthy boundaries

---

#### E3: The Achiever
**Core Motivation**: Want to be valuable, to be successful, to be admired

**Starting Attribute Modifiers**:
- Ambition +15 (goal-oriented)
- Drive +15 (highly motivated)
- Compassion -10 (deprioritizes relationships for success)

**Win Condition**:
- main_story_progress ≥ 90 AND social_capital ≥ 70 AND Self-Assurance ≥ 70
- (True success with authentic self-worth)

**Character Arc**: Finding authentic identity beyond achievements

---

#### E4: The Individualist
**Core Motivation**: Want to find themselves, to discover their significance, to be unique

**Starting Attribute Modifiers**:
- Ambition +10 (seeking meaning)
- Discernment +10 (introspective)
- Self-Assurance -10 (feels something is missing)

**Win Condition**:
- character_insight ≥ 80 AND main_story_progress complete AND Self-Assurance ≥ 65
- (Found significance through self-understanding)

**Character Arc**: Discovering self-worth isn't dependent on being special

---

#### E5: The Investigator
**Core Motivation**: Want to be capable, to be competent, to understand

**Starting Attribute Modifiers**:
- Discernment +15 (analytical)
- Ambition +5 (intellectual curiosity)
- Compassion -10 (emotionally detached)
- Drive -5 (conserves energy)

**Win Condition**:
- secrets_learned ≥ 20 AND Discernment ≥ 80 AND Compassion ≥ 50
- (Mastery with connection to others)

**Character Arc**: Engaging with the world rather than just observing

---

#### E6: The Loyalist
**Core Motivation**: Want to be secure, to be supported, to have certainty

**Starting Attribute Modifiers**:
- Discernment +10 (cautious, vigilant)
- Bravery -10 (anxious, risk-averse)
- Compassion +5 (loyal to others)

**Win Condition**:
- faction_standing ≥ 70 AND Bravery ≥ 60 AND psychological_strain < 30
- (Found security through courage and community)

**Character Arc**: Trusting self and overcoming anxiety

---

#### E7: The Enthusiast
**Core Motivation**: Want to be happy, to be satisfied, to be content

**Starting Attribute Modifiers**:
- Ambition +10 (many interests)
- Drive +10 (high energy)
- Discernment -10 (avoids negative)
- Bravery +5 (adventurous)

**Win Condition**:
- psychological_strain < 20 AND social_capital ≥ 60 AND Discernment ≥ 55
- (Found contentment through facing difficulties)

**Character Arc**: Finding satisfaction in depth rather than breadth

---

#### E8: The Challenger
**Core Motivation**: Want to be strong, to be in control, to be independent

**Starting Attribute Modifiers**:
- Bravery +15 (confrontational)
- Drive +15 (dominating)
- Self-Assurance +10 (confident)
- Compassion -15 (tough exterior)

**Win Condition**:
- Bravery ≥ 70 AND Compassion ≥ 60 AND enemies_made < 5
- (Strength with vulnerability and connection)

**Character Arc**: Allowing vulnerability and gentleness

---

#### E9: The Peacemaker
**Core Motivation**: Want to be at peace, to avoid conflict, to maintain harmony

**Starting Attribute Modifiers**:
- Compassion +10 (empathetic)
- Drive -15 (passive, avoids stirring things up)
- Discernment -5 (sees all sides)
- Bravery -5 (conflict-averse)

**Win Condition**:
- Drive ≥ 60 AND psychological_strain < 25 AND social_capital ≥ 70
- (Found peace through presence and healthy assertion)

**Character Arc**: Engaging actively rather than numbing out

---

### Drive Implementation Notes

**Character Creation**: Player chooses one Drive, which:
1. Applies attribute modifiers to starting values (base 50 + modifiers)
2. Sets the character's win condition
3. Stores Drive enum value on Character model

**During Gameplay**:
- Drive can be referenced in storylet prerequisites (e.g., "Only available to The Reformer")
- Some storylets might specifically address Drive themes
- Win condition is evaluated to determine character arc completion

**Example Starting Values** (The Reformer):
- Self-Assurance: 50 - 5 = 45
- Compassion: 50 + 10 = 60
- Ambition: 50 (no modifier)
- Drive: 50 (no modifier)
- Discernment: 50 + 15 = 65
- Bravery: 50 (no modifier)

---

## Core Attributes (The 6 Personality Scales)

### Overview
Psyche uses **NO traditional RPG stats** (no HP, Strength, Defense, etc.). Instead, characters are entirely defined by 6 personality attributes. Each attribute is a scale from 0-100, where:
- **0** = the negative extreme (left)
- **50** = the balanced center
- **100** = the excessive extreme (right)

### The 6 Attributes

1. **Self-Assurance**: `[inadequacy ↔ self-assurance ↔ arrogance]`
   - 0-30: Inadequate, lacks confidence, self-doubt
   - 40-60: Balanced self-assurance, healthy confidence
   - 70-100: Arrogant, overconfident, dismissive of others

2. **Compassion**: `[unempathetic ↔ compassionate ↔ lack of boundaries]`
   - 0-30: Unempathetic, cold, self-centered
   - 40-60: Compassionate with healthy boundaries
   - 70-100: Overly empathetic, lack of personal boundaries, easily manipulated

3. **Ambition**: `[unimaginative ↔ ambitious ↔ fantasist]`
   - 0-30: Unimaginative, uninspired, lacks goals
   - 40-60: Healthy ambition, realistic goals
   - 70-100: Fantasist, unrealistic expectations, chasing impossible dreams

4. **Drive**: `[passive ↔ driven ↔ steamroller]`
   - 0-30: Passive, unmotivated, reactive
   - 40-60: Balanced drive, persistent but considerate
   - 70-100: Steamroller, ruthlessly driven, bulldozes obstacles and people

5. **Discernment**: `[gullible ↔ discerning ↔ overly critical]`
   - 0-30: Gullible, naive, accepts everything
   - 40-60: Discerning, good judgment, appropriately skeptical
   - 70-100: Overly critical, cynical, rejects everything

6. **Bravery**: `[selfish ↔ brave ↔ reckless]`
   - 0-30: Selfish, cowardly, self-preserving at others' expense
   - 40-60: Brave, courageous with appropriate caution
   - 70-100: Reckless, foolhardy, endangers self and others

### How Attributes Work

**Dual Impact**: All 6 attributes affect BOTH:
- **Combat** (mechanics TBD): How characters perform in battles
- **Storylets**: Which narrative content becomes available

**Dynamic Changes**: Attributes change through:
- Battle outcomes (victory/defeat, tactics used)
- Storylet effects (narrative choices)
- Character Drives (starting values)

**No "Best" Value**: The extremes (0 or 100) aren't necessarily better than middle values. Balance (around 50) is often optimal, but different storylets reward different values.

---

## Attributes in Combat

**Status**: Combat mechanics are currently **TBD** (To Be Determined)

**Planned Approach**: The 6 personality attributes will determine combat effectiveness, but the specific mechanics haven't been finalized.

**Potential Mechanics** (under consideration):
- Bravery might affect offensive capability or willingness to take risks
- Discernment might improve tactical decision-making
- Drive might determine initiative or persistence in extended battles
- Self-Assurance might affect ability to withstand pressure
- Compassion might enable support/healing actions
- Ambition might influence strategic planning

**Important**: Since there's no HP stat, defeat leads to **narrative consequences** (not death):
- Attribute shifts (Self-Assurance decreases, psychological_strain increases)
- Story changes (enemies_made increases, new menace storylets unlock)
- Character development (crisis moments that drive the narrative forward)

---

## Attributes in Storylets

Attributes directly gate narrative content availability. Storylets can require specific attribute values to appear.

**Example Prerequisites**:
- "A Moment of Courage" requires Bravery ≥ 60
- "Questioning Authority" requires Discernment ≥ 70
- "Empathetic Connection" requires Compassion ≥ 60 AND Compassion < 80 (not too much)
- "Balanced Approach" requires ALL attributes between 40-60

**Emergent Storytelling**:
- A character with high Bravery (75) might see aggressive, confrontational storylets
- A character with high Compassion (70) might see opportunities for reconciliation
- A character with high psychological_strain (80) AND low Self-Assurance (30) might face crisis storylets

**Attribute Changes from Storylets**:
- Storylets can modify attributes as effects
- Example: "Face Your Fears" might increase Bravery by +5 and Self-Assurance by +3
- Different choices within storylets can shift attributes in different directions

---

## Attributes vs Qualities

The system uses **two types of stats**:

### Core Attributes (6 personality scales)
- **What**: Self-Assurance, Compassion, Ambition, Drive, Discernment, Bravery
- **Range**: 0-100 (scales with negative/balanced/excessive extremes)
- **Purpose**: Define WHO the character is
- **Storage**: Direct properties on Character model
- **Affects**: Both combat and storylets
- **Changes**: Through battles, storylets, character development

### Storylet Qualities (narrative stats)
- **What**: Additional stats like reputation, stress, story progress
- **Range**: Varies by quality (often 0-100 or unbounded counters)
- **Purpose**: Track WHAT has happened in the narrative
- **Storage**: QualityCollection on Character model
- **Affects**: Primarily storylets (some might affect combat indirectly)
- **Changes**: Through storylet effects and battle outcomes

### Defined Qualities for MVP:
**Progress Qualities**:
- `main_story_progress`: Narrative arc advancement (0-100)
- `character_insight`: Understanding of psychology (0-100)
- `faction_standing`: Relationship with groups (0-100)

**Menace Qualities**:
- `psychological_strain`: Mental/emotional stress (0-100, higher is worse)
- `enemies_made`: Antagonist attention (counter, unbounded)

**Resource Qualities**:
- `social_capital`: Influence and connections (0-100, spendable)
- `secrets_learned`: Information advantage (counter)

**Metric Qualities**:
- `battles_experienced`: Combat encounters (counter)
- `key_choices_made`: Major decisions (counter)

### How They Interact
- **Storylets can gate on BOTH**: "Requires Bravery ≥ 60 AND social_capital ≥ 10"
- **Storylets can affect BOTH**: "Increases Compassion by +3, decreases psychological_strain by -10"
- **Emergent interplay**: High Bravery with high psychological_strain creates different opportunities than balanced Bravery with low strain
- **Different purposes**: Attributes define personality, qualities track narrative state

---

## System Integration: How Everything Works Together

### The Complete Picture

Psyche uses a unified character system where everything interconnects:

```
                    ┌─────────────┐
                    │   DRIVES    │
                    │ (Enneagram) │
                    └──────┬──────┘
                           │
                    Affects Starting Values
                    Sets Win Condition
                           │
                           ▼
              ┌────────────────────────┐
              │   CORE ATTRIBUTES      │
              │  (6 personality scales) │
              └───┬────────────────┬───┘
                  │                │
         Affects  │                │  Modified by
         Combat & │                │  Storylets & Battles
         Storylets│                │
                  │                │
                  ▼                ▼
         ┌────────────┐    ┌──────────────┐
         │  BATTLES   │    │  STORYLETS   │
         │ (outcomes) │◀──▶│  (choices)   │
         └──────┬─────┘    └──────┬───────┘
                │                  │
                └────────┬─────────┘
                         │
                    Modify Both
                         │
                         ▼
                ┌─────────────────┐
                │STORYLET QUALITIES│
                │ (narrative stats)│
                └─────────┬────────┘
                          │
                    Gate New Storylets
                    Track Progress
                          │
                          ▼
                  ┌──────────────┐
                  │ WIN CONDITION│
                  │  (per Drive) │
                  └──────────────┘
```

### Game Flow Example

**Character Creation**:
1. Player chooses **Drive**: "The Reformer"
2. Base attributes start at 50
3. Drive modifiers applied: Compassion +10 → 60, Discernment +15 → 65, Self-Assurance -5 → 45
4. Win condition set: Compassion ≥ 70 AND Discernment ≥ 70 AND psychological_strain < 30
5. Storylet qualities initialized at 0

**Early Game - Battle**:
1. Battle occurs (mechanics TBD)
2. Victory → Bravery +2 (47 → 49), Self-Assurance +1 (45 → 46)
3. Battle outcome → battles_experienced +1, social_capital +1

**Mid Game - Storylet Unlocked**:
1. Storylet "A Moral Dilemma" becomes available
2. Prerequisites met: Compassion ≥ 60 ✓, Discernment ≥ 65 ✓
3. Player chooses storylet
4. Content displays narrative choice
5. Player chooses "merciful path"
6. Effects applied: Compassion +5 (60 → 65), psychological_strain -5, character_insight +10

**Late Game - Crisis Point**:
1. Multiple battles lost → psychological_strain +40 (now at 65)
2. Self-Assurance dropped → 35 (from defeats)
3. New crisis storylets unlock: "Breaking Point" (requires psychological_strain ≥ 60)
4. Player must resolve crisis through storylet choices
5. Resolution: psychological_strain -30, character_insight +20, Self-Assurance +10

**End Game - Win Condition**:
1. Check Drive win condition: Compassion ≥ 70 ✓, Discernment ≥ 70 ✓, psychological_strain < 30 ✓
2. Character arc complete - The Reformer achieved balanced goodness

### Key Integration Points

**1. Drives → Attributes**:
- One-time modifier at character creation
- Shapes initial playstyle and storylet availability
- Creates different starting narrative opportunities

**2. Attributes ↔ Combat**:
- Attributes affect battle performance (mechanics TBD)
- Battle outcomes modify attributes
- No death - defeat causes attribute shifts and quality changes

**3. Attributes ↔ Storylets**:
- Attributes gate which storylets appear
- Storylet choices modify attributes
- Creates emergent character development

**4. Attributes + Qualities → Storylets**:
- Storylets can require BOTH types
- Example: "Requires Bravery ≥ 60 AND social_capital ≥ 10"
- Creates complex narrative gating

**5. Storylets → Qualities**:
- Storylets primarily modify qualities
- Tracks narrative progress and state
- Unlocks new storylets based on quality values

**6. Everything → Win Condition**:
- Win conditions check both attributes and qualities
- Different for each Drive
- Represents completing character's psychological arc

### Design Philosophy

**Emergent Narrative**: The interplay between attributes, qualities, battles, and storylets creates unique stories for each playthrough.

**Psychological Focus**: Character development is psychological growth (attributes shifting, insight increasing, strain managing) rather than power progression.

**No "Optimal" Build**: Different attribute configurations unlock different content. A character with extreme values experiences different storylets than a balanced character.

**Mechanical Simplicity, Narrative Complexity**: Simple rules (attributes 0-100, qualities track state) create complex emergent stories.

---

## Implementation Notes for Developers

### Character Model Structure
```csharp
public class Character
{
    // Core Attributes (0-100)
    public int SelfAssurance { get; set; }
    public int Compassion { get; set; }
    public int Ambition { get; set; }
    public int Drive { get; set; }
    public int Discernment { get; set; }
    public int Bravery { get; set; }

    // Drive System
    public DriveType ChosenDrive { get; set; }

    // Storylet Qualities
    public QualityCollection Qualities { get; set; }

    // Tracking
    public HashSet<string> PlayedStoryletIds { get; set; }
}
```

### Prerequisites Can Check Both
```csharp
// Check core attribute
new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 }

// Check storylet quality
new QualityRequirement { QualityId = "social_capital", MinValue = 10 }

// Check both with compound
new CompoundPrerequisite {
    Logic = And,
    Prerequisites = [
        new AttributeRequirement { AttributeName = "Bravery", MinValue = 60 },
        new QualityRequirement { QualityId = "social_capital", MinValue = 10 }
    ]
}
```

### Effects Can Modify Both
```csharp
// Modify core attribute
new AttributeEffect { AttributeName = "Bravery", Delta = +5 }

// Modify storylet quality
new QualityEffect { QualityId = "psychological_strain", Delta = -10 }

// Modify both with compound
new CompoundEffect {
    Effects = [
        new AttributeEffect { AttributeName = "Compassion", Delta = +3 },
        new QualityEffect { QualityId = "enemies_made", Delta = -1 },
        new QualityEffect { QualityId = "main_story_progress", Delta = +1 }
    ]
}
```

---

**Last Updated**: 2025-11-17
**Status**: System Design Complete, Ready for Implementation
