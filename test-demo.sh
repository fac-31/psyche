#!/bin/bash
# Run tests with detailed output to see the storylet system in action

echo "Running Psyche storylet tests with detailed output..."
echo "======================================================"
echo ""

dotnet test Tests/Psyche.Tests.csproj --logger "console;verbosity=detailed"
