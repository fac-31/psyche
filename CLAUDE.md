# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Psyche is a C# console application targeting .NET 9.0 with nullable reference types and implicit usings enabled. Currently in early development stage with minimal implementation.

## Build and Development Commands

### Building
```bash
dotnet build
```

### Running
```bash
dotnet run
```

### Testing
```bash
dotnet test
```

### Testing with Storylet Demo Output
See the storylet system in action with detailed output showing character state changes, prerequisite evaluation, and effect application:
```bash
./test-demo.sh
```

### Clean
```bash
dotnet clean
```

### Running a single test
```bash
dotnet test --filter "FullyQualifiedName~TestNamespace.TestClass.TestMethod"
```

## Project Configuration

- **Target Framework**: .NET 9.0
- **Project Type**: Console Application (Exe)
- **Features**: ImplicitUsings enabled, Nullable reference types enabled

## Custom Slash Commands

This project uses several custom slash commands for development workflow:

- `/explain [feature or workflow]` - Provides detailed step-by-step explanations of how a feature works
- `/report:project [optional: preferred focus]` - Analyzes roadmap files in `docs/dev/roadmap/` and provides detailed project status report
- `/report:module [Submodule Name]` - Analyzes specific module roadmap file at `docs/dev/roadmap/{module}-MVP.md` and summarizes its content
- `/update:docs [file to focus on]` - Updates project documentation as needed
- `/git:commit` - Creates git commits following project conventions
- `/git:pull-request` - Creates pull requests for the current branch

## Expected Project Structure

Based on the slash commands, the project expects:
- `docs/dev/roadmap/` - Directory containing roadmap documentation files
- Module-specific roadmap files named `{module}-MVP.md` within the roadmap directory
