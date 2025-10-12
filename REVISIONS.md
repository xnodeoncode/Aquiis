# Aquiis - Revision History

## October 12, 2025

### Migration Successfully Created and Applied

**Database Migration: AddPropertyTable**

- ✅ Created migration for Property entity with complete schema
- ✅ Added ApplicationUser tracking fields (LastLoginDate, PreviousLoginDate, LoginCount, LastLoginIP)
- ✅ Database successfully created at `./Data\app.db`
- ✅ All tables, indexes, and constraints applied correctly
- ✅ Schema validated and matches entity models

**Changes Include:**

- Property table with full property management fields
- Enhanced user login tracking capabilities
- Automatic database creation on application startup
- Resolved EF Core tools installation issues by using manual migration approach

### Database Creation Scripts Added

**Database Scripts: Data/Scripts Directory**

- ✅ Created comprehensive SQL scripts for manual database creation
- ✅ Added `01_CreateTables.sql` with complete table structure and constraints
- ✅ Added `02_CreateIndexes.sql` with performance optimization indexes
- ✅ Added `03_SeedData.sql` with default roles and optional sample data
- ✅ Added `README.md` with complete documentation and usage instructions

**Script Features:**

- SQLite optimized syntax and data types
- Production ready with foreign key constraints
- Performance indexes for common query patterns
- Default roles (Administrator, PropertyManager, Tenant)
- Comprehensive documentation and usage examples
- Backup method for database creation alongside EF migrations

### VS Code Debugging Configuration Added

**Debug Setup: .vscode Directory and Workspace Configuration**

- ✅ Created `.vscode/launch.json` with comprehensive debug configurations
- ✅ Created `.vscode/tasks.json` with build and development tasks
- ✅ Updated `Aquiis.code-workspace` with enhanced workspace settings

**Created Files:**

### 1. `.vscode/launch.json` - Debug Configurations

- **Launch Aquiis.WebUI (Development)** - Standard debugging with auto browser opening
- **Launch Aquiis.WebUI (Production)** - Production environment debugging
- **Attach to Aquiis.WebUI** - Attach to running process

### 2. `.vscode/tasks.json` - Build Tasks

- **build** - Standard debug build (default)
- **build-release** - Release build
- **watch** - Hot reload development mode
- **publish** - Production publish
- **clean** - Clean build artifacts

### 3. Updated `Aquiis.code-workspace` - Enhanced Workspace

- **Settings**: Default solution, file exclusions, OmniSharp configuration
- **Extensions**: Recommended C#, .NET, and web development extensions
- **Launch Configuration**: Embedded debug configuration for workspace-level debugging

**Key Features:**

- ✅ **Auto Browser Opening** - Automatically opens browser when debugging starts
- ✅ **Environment Variables** - Proper ASPNETCORE settings for each configuration
- ✅ **Pre-Launch Tasks** - Automatically builds before debugging
- ✅ **Multiple Configurations** - Development, Production, and Attach modes
- ✅ **Hot Reload Support** - Watch task for rapid development
- ✅ **Workspace Integration** - Launch configs available at workspace level

**Usage:**

- **Press F5** to start debugging with the default configuration
- **Ctrl+Shift+P** → "Tasks: Run Task" to run specific build tasks
- **Debug Panel** (Ctrl+Shift+D) to select different launch configurations
- **Watch Mode**: Run the "watch" task for hot reload development
