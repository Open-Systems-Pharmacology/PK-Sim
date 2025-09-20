# PK-Sim Development Instructions

PK-Sim® is a comprehensive .NET-based physiologically based pharmacokinetic (PBPK) modeling software for whole-body simulations. It consists of multiple components including a GUI application, command-line interface (CLI), and various supporting libraries.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Critical Prerequisites

**NEVER CANCEL BUILDS OR LONG-RUNNING COMMANDS**. Builds may take 45+ minutes, tests may take 15+ minutes. Use timeouts of 60+ minutes for builds and 30+ minutes for tests.

### Required Dependencies
- .NET 8.0 SDK (verified working with 8.0.119) - **REQUIRED**
- Ruby (for build scripts and automation) - verified working with Ruby 3.2.3
- Git with submodule support - **REQUIRED** 
- **OSPSuite GitHub packages access** - **CRITICAL REQUIREMENT** (requires GitHub authentication token)
- SQLite3 command-line tools (for database operations)

## Repository Setup

Initialize all required submodules:
```bash
git submodule update --init --recursive
```

This downloads essential components:
- `scripts/` - Ruby build automation scripts
- `dimensions/` - Dimension definitions (XML)
- `documentation/` - Documentation files
- `pkparameters/` - PK parameter definitions
- `templates/` - Building block templates

**NEVER CANCEL** - Submodule initialization may take 5+ minutes.

Verify submodules are properly initialized:
```bash
git submodule status
```
Should show 5 submodules: dimensions, documentation, pkparameters, scripts, templates.

## Build System

### GitHub Packages Authentication **REQUIRED**

The build requires OSPSuite packages (version 12.1.128) from GitHub packages:
- OSPSuite.Core
- OSPSuite.Infrastructure  
- OSPSuite.Presentation
- OSPSuite.Assets
- OSPSuite.Utility
- And many others

Add GitHub packages source:
```bash
dotnet nuget add source "https://nuget.pkg.github.com/Open-Systems-Pharmacology/index.json" \
  --name "OSP-GitHub-Packages" \
  --username "Open-Systems-Pharmacology" \
  --password "$GITHUB_TOKEN" \
  --store-password-in-clear-text
```

**WITHOUT GITHUB TOKEN**: Build will fail with NU1102 errors. You cannot build the solution without proper GitHub packages access.

**Alternative approaches when GitHub token is unavailable:**
- Contact repository maintainers for access  
- Use pre-built binaries if available
- Focus only on documentation/configuration changes  
- Use separate test projects to validate .NET environment

### Primary Build Commands

Restore dependencies:
```bash
dotnet restore
```
**TIMEOUT: 15+ minutes**. Package restoration from GitHub can be slow.

Build solution:
```bash
dotnet build PKSim.sln /p:Version=12.1.9999
```
**NEVER CANCEL - Build takes 45+ minutes**. Set timeout to 60+ minutes minimum.

## Testing

Run all tests:
```bash
dotnet test .\tests\**\bin\Debug\net472\PKSim*Tests.dll -v normal --no-build --logger:"html;LogFileName=../testLog.html"
```
**NEVER CANCEL - Tests take 15+ minutes**. Set timeout to 30+ minutes minimum.

Test projects available:
- `PKSim.Tests` - Core functionality tests
- `PKSim.UI.Tests` - User interface tests  
- `PKSim.R.Tests` - R integration tests
- `PKSim.Matlab.Tests` - MATLAB integration tests

## Command-Line Interface (CLI)

The CLI application (`src/PKSim.CLI/`) supports multiple commands:

### Available Commands
- `json` - JSON-based simulation runs
- `snapshot` - Snapshot operations
- `export` - Export operations
- `qualification` - Qualification workflow runs

### CLI Usage Examples
```bash
# Show help
PKSim.CLI.exe --help

# Qualification run
PKSim.CLI.exe qualification --input config.json --validate --run --exp

# Export operations
PKSim.CLI.exe export --input input.json --output results.json
```

### CLI Command Structure
- All commands inherit from `CLICommand` base class
- Support logging with `--log` and `--logLevel` options
- Return exit codes: Success=0, Error=1

## Project Structure

### Key Projects
- **`src/PKSim/`** - Main GUI application (WPF-based, targets net472/net8)
- **`src/PKSim.CLI/`** - Command-line interface application (targets net472/net8) 
- **`src/PKSim.Core/`** - Core business logic and models (netstandard2.0)
- **`src/PKSim.Infrastructure/`** - Data access and infrastructure (netstandard2.0)
- **`src/PKSim.Presentation/`** - Presentation layer (netstandard2.0)
- **`src/PKSim.Assets/`** - Shared resources (netstandard2.0)
- **`src/PKSim.Assets.Images/`** - Image resources (netstandard2.0)
- **`src/PKSim.R/`** - R integration components (netstandard2.0)
- **`src/Db/PKSimDB.sqlite`** - Main SQLite database (51MB)

### Test Projects
- **`tests/PKSim.Tests/`** - Core functionality unit tests
- **`tests/PKSim.UI.Tests/`** - User interface tests
- **`tests/PKSim.R.Tests/`** - R integration tests  
- **`tests/PKSim.Matlab.Tests/`** - MATLAB integration tests

### Build Targets
- **net472** - .NET Framework 4.7.2 (primary Windows target, used in CI)
- **net8** - .NET 8.0 (cross-platform target)
- **netstandard2.0** - .NET Standard 2.0 (for shared libraries)

### Submodule Components
- **`scripts/`** - Ruby build automation scripts (from Open-Systems-Pharmacology/build-scripts)
- **`dimensions/`** - OSPSuite.Dimensions.xml definitions
- **`documentation/`** - User documentation and PDFs
- **`pkparameters/`** - OSPSuite.PKParameters.xml definitions  
- **`templates/`** - Building block templates (templates.json with compound library URLs)

## Database Operations

Database located at: `src/Db/PKSimDB.sqlite`

The database contains core PK-Sim model definitions including:
- Species definitions (`tab_species`)
- Process types and kinetics (`tab_process_types`, `tab_kinetic_types`) 
- Application types (`tab_application_types`)
- Dimensions and parameters (`tab_dimensions`)
- Reference data (`tab_references`)

### Database Tasks (Linux/Cross-platform)
```bash
# View database tables
sqlite3 src/Db/PKSimDB.sqlite "SELECT name FROM sqlite_master WHERE type='table';"

# Export database structure
sqlite3 src/Db/PKSimDB.sqlite ".schema"

# Examine specific table
sqlite3 src/Db/PKSimDB.sqlite "SELECT * FROM tab_species LIMIT 5;"
```

### Database Ruby Tasks (Windows-only)
These require Windows sqlite3.exe tools in `src/Db/Dump/` and `src/Db/Diff/`:
```bash
rake db:dump    # Export database to text file (Windows only)
rake db:diff    # Compare with develop branch database (Windows only)
```

**Note**: The Ruby database tasks use Windows-only executables that won't run on Linux/Mac.

## Validation Requirements

### Environment Validation
First verify your development environment:
```bash
# Check .NET version (should be 8.0+)
dotnet --version

# Verify Ruby (should be 3.0+)
ruby --version

# Test SQLite access
sqlite3 --version

# Verify submodules are initialized
git submodule status
```

### .NET Environment Test
Create a simple test project to verify .NET build environment:
```bash
cd /tmp && mkdir test-build && cd test-build
dotnet new console -n TestApp
cd TestApp && dotnet build && dotnet run
```
**Expected**: Build succeeds in ~10 seconds, outputs "Hello, World!"

### Build Validation (Requires GitHub Packages Access)
Always run these validation steps after making changes:
1. Restore dependencies (15+ min timeout) - **NEVER CANCEL**
2. Build solution (60+ min timeout) - **NEVER CANCEL**
3. Run relevant tests (30+ min timeout) - **NEVER CANCEL** 
4. Test CLI functionality if modified

### Manual Testing Scenarios
**CRITICAL**: After making changes, validate:

1. **CLI Functionality**: 
   - Test CLI help: `PKSim.CLI.exe --help`
   - Validate command parsing works correctly
   - Test with sample configuration files

2. **Application Startup**:
   - Verify main application starts without crashes
   - Check database connectivity
   - Validate critical workflows

## Ruby Build Automation

Rake tasks available:
```bash
rake --tasks                    # List all tasks
rake create_setup               # Create installer setup
rake cover                      # Run code coverage analysis
```

Coverage targets:
- PKSim.Core
- PKSim.Assets
- PKSim.Presentation  
- PKSim.Infrastructure

## Common Issues and Workarounds

### Package Restore Failures
**Symptom**: NU1102 errors for OSPSuite packages
**Solution**: Ensure GitHub packages authentication is configured correctly

### Long Build Times
**Expected**: 45+ minute builds are normal due to complex dependencies
**Action**: Always set appropriate timeouts, never cancel builds

### Submodule Issues
**Symptom**: Missing scripts or build failures
**Solution**: Re-run `git submodule update --init --recursive`

## CI/CD Pipeline

GitHub Actions workflow (`.github/workflows/build-and-test.yml`):
- Runs on Windows (required for full compatibility)
- Uses `dotnet restore` with GitHub packages authentication
- Builds with version parameter
- Runs tests against .NET Framework 4.7.2 binaries
- Creates R dependency artifacts

## File Locations - Quick Reference

```
/
├── PKSim.sln                    # Main solution file
├── src/
│   ├── PKSim/                   # Main application
│   │   ├── PKSim.csproj         # Main app project file
│   │   └── app.config           # App configuration
│   ├── PKSim.CLI/               # Command-line interface  
│   │   ├── PKSim.CLI.csproj     # CLI project file
│   │   ├── Program.cs           # CLI entry point
│   │   ├── Commands/            # CLI command implementations
│   │   └── app.config           # CLI configuration
│   ├── PKSim.Core/              # Core business logic
│   │   └── PKSim.Core.csproj    # Core library project
│   └── Db/
│       ├── PKSimDB.sqlite       # Main database (51MB)
│       ├── Dump/sqlite3.exe     # Windows DB tools
│       └── Diff/sqldiff.exe     # Windows diff tools
├── tests/                       # All test projects
│   ├── PKSim.Tests/             # Core tests
│   ├── PKSim.UI.Tests/          # UI tests
│   ├── PKSim.R.Tests/           # R integration tests
│   └── PKSim.Matlab.Tests/      # MATLAB tests
├── scripts/                     # Ruby build scripts (submodule)
│   ├── setup.rb                 # Setup automation
│   ├── coverage.rb              # Code coverage
│   └── utils.rb                 # Utility functions
├── dimensions/                  # Dimension definitions (submodule)
│   └── OSPSuite.Dimensions.xml  # Dimension specifications
├── documentation/               # Documentation (submodule)
├── pkparameters/                # PK parameter definitions (submodule)
├── templates/                   # Building block templates (submodule)
│   └── templates.json           # Compound library URLs
├── examples/                    # Example files
│   └── README.txt               # Points to external example repository
└── .github/
    ├── workflows/               # CI/CD pipelines
    │   ├── build-and-test.yml   # Main CI workflow
    │   └── coverage.yml         # Coverage workflow
    └── copilot-instructions.md  # This file
```

## Working Effectively

1. **Always initialize submodules first** before any development work
2. **Set long timeouts** for all build and test operations
3. **Verify GitHub packages access** before attempting builds
4. **Test CLI commands manually** after any CLI-related changes
5. **Use Ruby rake tasks** for advanced build operations and setup creation
6. **Check database connectivity** when working with data-related features

Remember: This is a complex PBPK modeling application with significant build requirements. Plan for longer development cycles and ensure proper authentication setup before beginning work.