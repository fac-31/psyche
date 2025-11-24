# Psyche - Development Roadmap

This directory contains the development roadmaps for the Psyche project, a turn-based RPG system built with C# and .NET 9.0.

## Project Overview

Psyche is a turn-based role-playing game featuring character progression, tactical combat, and a rich equipment and skill system. The project is currently in early development with roadmaps defined for core systems.

## Roadmap Files

### [Psyche.md](./Psyche.md) - Project Hub
Main project roadmap covering:
- Overall project milestones and integration
- Cross-module features
- Project-wide tasks and goals

### [character-mvp.md](./character-mvp.md) - Character System
Character system roadmap including:
- **M1**: Core Character Stats (HP, Strength, Defense, Speed)
- **M2**: Skills and Abilities (skill system with MP costs and effects)
- **M3**: Equipment System (weapons, armor, accessories)
- **M4**: Inventory Management (item storage and usage)
- **M5**: Character Progression (XP, leveling, stat growth)

### [battleSystem-mvp.md](./battleSystem-mvp.md) - Battle System
Turn-based combat system roadmap including:
- **M1**: Core Battle Loop (turn management, action selection)
- **M2**: Combat Actions (attack, defend, skills, items)
- **M3**: Enemy AI (decision-making, behavior patterns)
- **M4**: Battle UI (text-based display, menus, visualization)

### [storylets-MVP.md](./storylets-MVP.md) - Storylet System
Quality-based narrative system roadmap including:
- **M1**: Quality System (core attributes and storylet qualities)
- **M2**: Storylet Core Engine ✅ (prerequisites, effects, validation)
- **M3**: Storylet Management ✅ (repository, JSON storage, availability)
- **M4**: Integration & UI (console interface, game loop integration)
- **M5**: Content & Testing 🔄 (demo storylets, comprehensive test suite)

## Current Status

**Phase**: Core Systems Implementation

**Completed**:
- ✅ Project initialization (.NET 9.0)
- ✅ Roadmap documentation for core modules
- ✅ Module structure defined
- ✅ Unit testing framework (xUnit with extensive test coverage)
- ✅ **Storylet Core Engine (M2)** - Prerequisites, effects, options, validation
- ✅ **Storylet Management (M3)** - Repository pattern, JSON/JSONC storage, type-safe serialization
- ✅ Demo storylets and interactive walkthrough

**In Progress**:
- 🔄 Storylet system content creation (M5) - 2/15 demo storylets complete
- Character system implementation
- Battle system implementation

**Next Steps**:
1. Complete remaining demo storylets (8-13 more needed)
2. Implement Storylet UI (M4)
3. Integrate storylet system with character and battle systems
4. Implement Character core stats (M1)
5. Implement Battle core loop (M1)

## Development Workflow

1. Check module-specific roadmaps for detailed tasks
2. Use `/report:module [module-name]` to get status reports
3. Use `/report:project` for overall project status
4. Reference [CLAUDE.md](../../../CLAUDE.md) for development commands

## Beyond MVP

All systems have extensive post-MVP features planned:
- **Character**: Multiple classes, advanced customization, character relationships
- **Battle**: Party-based combat, status effects, environmental mechanics, visual enhancements
- **Storylets**: Advanced prerequisites, storylet chains/decay, relationship system, faction reputation, content creation tools
- **Integration**: World map, NPC system, quest system, save/load functionality
