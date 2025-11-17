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

## Current Status

**Phase**: Planning and Initial Development

**Completed**:
- ✅ Project initialization (.NET 9.0)
- ✅ Roadmap documentation for core modules
- ✅ Module structure defined

**In Progress**:
- Character system implementation
- Battle system implementation

**Next Steps**:
1. Set up unit testing framework
2. Implement Character core stats (M1)
3. Implement Battle core loop (M1)
4. Begin module integration

## Development Workflow

1. Check module-specific roadmaps for detailed tasks
2. Use `/report:module [module-name]` to get status reports
3. Use `/report:project` for overall project status
4. Reference [CLAUDE.md](../../../CLAUDE.md) for development commands

## Beyond MVP

Both systems have extensive post-MVP features planned:
- **Character**: Multiple classes, advanced customization, character relationships
- **Battle**: Party-based combat, status effects, environmental mechanics, visual enhancements
- **Integration**: World map, NPC system, quest system, save/load functionality
