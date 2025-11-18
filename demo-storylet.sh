#!/bin/bash
# Demonstrates the complete storylet choice flow
# Shows a character encountering a storylet, viewing options, choosing, and experiencing effects

echo ""
echo "╔════════════════════════════════════════════════════════════════════════════╗"
echo "║                     PSYCHE STORYLET SYSTEM DEMO                            ║"
echo "╚════════════════════════════════════════════════════════════════════════════╝"
echo ""
echo "This demo shows the complete flow of the quality-based narrative system:"
echo "  1. Creating a character with specific attributes and qualities"
echo "  2. Loading storylets from the repository"
echo "  3. Encountering a storylet and reading its content"
echo "  4. Evaluating which options are available based on prerequisites"
echo "  5. Making a choice"
echo "  6. Applying the consequences (effects)"
echo "  7. Seeing how the character has changed"
echo ""
echo "Press Enter to begin the demonstration..."
read

dotnet test Tests/Psyche.Tests.csproj \
  --filter "FullyQualifiedName~StoryletWalkthroughDemo.CompleteStoryletWalkthrough_FromEncounterToConsequence" \
  --logger "console;verbosity=detailed" \
  --nologo \
  2>&1 | grep -v "^  Determining" | grep -v "^  All projects" | grep -v "^  psyche ->" | grep -v "^  Psyche.Tests ->" | grep -v "^Test run for" | grep -v "^VSTest" | grep -v "^Starting test" | grep -v "^A total of"

echo ""
