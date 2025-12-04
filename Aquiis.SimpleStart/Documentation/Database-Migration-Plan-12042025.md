# Database Migration Plan

**Date:** December 4, 2025

## Overview

Implement automatic database migration from previous schema versions to current version for Electron desktop application. This ensures seamless upgrades when users install new versions with schema changes.

## Configuration

The migration process uses two settings in `appsettings.json`:

- `DatabaseFileName`: Current database filename (e.g., `app_v0.1.0.db`)
- `PreviousDatabaseFileName`: Previous version database filename (e.g., `app.db`)

## Detection & Migration Flow

### 1. Startup Detection

On application startup, check database state:

- **If current database exists** → Normal startup (no migration needed)
- **If current database doesn't exist** → Check for previous database
  - **If previous database exists** → Trigger migration process
  - **If no databases exist** → New installation (create fresh database)

### 2. Migration Process

When migration is triggered:

1. **Backup Previous Database**

   - Create timestamped backup: `{previousDb}.backup.{timestamp}`
   - Ensures rollback capability if migration fails

2. **Create New Database**

   - Initialize current schema using `context.Database.MigrateAsync()`
   - Apply all current migrations

3. **Open Dual Connections**

   - `oldContext` → Connection to `PreviousDatabaseFileName`
   - `newContext` → Connection to current `DatabaseFileName`

4. **Copy Data Table-by-Table**

   - Process entities in dependency order (respect foreign keys)
   - Map old schema to new schema
   - Handle schema differences

5. **Cleanup**
   - Close old database connection
   - Rename old database: `{previousDb}.migrated.{timestamp}`
   - Log migration success

### 3. Implementation Location

**File:** `Program.cs`  
**Section:** Electron database initialization (around lines 198-210)  
**Timing:** Before `context.Database.MigrateAsync()` is called

## Technical Approach

### Database Connection Strategy

```csharp
// Pseudo-code structure
var currentDbPath = await pathService.GetDatabasePathAsync();
var previousDbPath = Path.Combine(
    Path.GetDirectoryName(currentDbPath),
    configuration["ApplicationSettings:PreviousDatabaseFileName"]
);

if (!File.Exists(currentDbPath) && File.Exists(previousDbPath))
{
    // Migration needed
    await MigrateDatabaseAsync(previousDbPath, currentDbPath);
}
```

### Entity Migration Order

Must respect foreign key dependencies:

1. **Independent Tables** (no dependencies):

   - `ChecklistTemplates`
   - `ChecklistTemplateItems`

2. **User & Organization Tables**:

   - `AspNetUsers`
   - `Organizations`
   - `UserOrganizations`

3. **Property Management Tables** (in order):

   - `Properties`
   - `Tenants`
   - `Leases`
   - `SecurityDeposits`
   - `LeaseDocuments`

4. **Related Entities**:
   - `Applications`
   - `Documents`
   - `Notes`
   - `CalendarEvents`
   - `ScheduledTasks`
   - `FinancialReports`
   - `Settings`

### Data Copying Strategy

```csharp
// Two DbContext instances with different connection strings
var oldContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlite($"DataSource={previousDbPath}")
    .Options;

var newContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlite($"DataSource={currentDbPath}")
    .Options;

using var oldContext = new ApplicationDbContext(oldContextOptions);
using var newContext = new ApplicationDbContext(newContextOptions);

// Copy data
var oldEntities = await oldContext.Set<Entity>().ToListAsync();
foreach (var entity in oldEntities)
{
    var newEntity = MapToNewSchema(entity);
    newContext.Set<Entity>().Add(newEntity);
}
await newContext.SaveChangesAsync();
```

## Challenges & Solutions

### 1. Schema Changes

**Challenge:** Columns renamed, removed, or changed types  
**Solution:** Implement mapping logic per entity type

```csharp
Entity MapToNewSchema(OldEntity old)
{
    return new Entity
    {
        Id = old.Id,
        NewPropertyName = old.OldPropertyName,
        RequiredField = old.OptionalField ?? "default"
    };
}
```

### 2. New Required Fields

**Challenge:** New schema has non-nullable fields not in old schema  
**Solution:**

- Provide sensible defaults
- Calculate from existing data
- Prompt user for critical missing data (rare)

### 3. Foreign Key Dependencies

**Challenge:** Must insert in correct order to avoid constraint violations  
**Solution:** Process tables in dependency order (see "Entity Migration Order")

### 4. Identity Column Preservation

**Challenge:** Auto-increment IDs may need to be preserved  
**Solution:**

- SQLite: Use `INSERT` with explicit IDs
- Reset sequence after migration: `UPDATE sqlite_sequence SET seq = (SELECT MAX(id) FROM table) WHERE name = 'table'`

### 5. Large Datasets

**Challenge:** Migration may take time with large databases  
**Solution:**

- Process in batches (e.g., 1000 records at a time)
- Show progress indicator (log percentage complete)
- Use transactions per batch for rollback capability

### 6. Migration Failures

**Challenge:** Migration fails mid-process  
**Solution:**

- Keep original database intact (only rename on success)
- Delete incomplete new database on failure
- Log detailed error information
- Allow user to retry or skip migration

## Progress Reporting

Log migration progress for user visibility:

```
[INFO] Database migration detected: app.db → app_v0.1.0.db
[INFO] Backing up previous database...
[INFO] Creating new database schema...
[INFO] Migrating Organizations... (5 records)
[INFO] Migrating Properties... (12 records)
[INFO] Migrating Tenants... (8 records)
[INFO] Migrating Leases... (15 records)
[INFO] Migration completed successfully
[INFO] Previous database archived: app.db.migrated.20251204153045
```

## Rollback Strategy

If migration fails:

1. Delete incomplete new database
2. Restore from backup if original was modified
3. Log detailed error for debugging
4. Continue with old database (don't block app startup)
5. Notify user that manual intervention may be needed

## Testing Strategy

Test scenarios:

1. **New installation** - No databases exist
2. **Normal startup** - Current database exists
3. **Migration needed** - Only previous database exists
4. **Schema changes** - Test with actual schema differences
5. **Large dataset** - Test with 10,000+ records
6. **Mid-migration failure** - Simulate error during copy
7. **Rollback** - Verify backup/restore works

## Future Enhancements

1. **UI Progress Dialog** - Show migration progress in Electron window
2. **Migration History Table** - Track all migrations performed
3. **Incremental Migrations** - Support multiple version jumps (v0.1 → v0.2 → v0.3)
4. **Data Validation** - Verify data integrity after migration
5. **Selective Migration** - Allow user to choose what data to migrate
6. **Export/Import** - Alternative to in-place migration

## Similar Implementations

This pattern is used by:

- **VS Code** - Extension database migrations
- **Slack Desktop** - User data migrations
- **iOS/Android Apps** - Core Data/Room migrations
- **Electron Apps** - Local storage upgrades

## Success Criteria

✅ Users never lose data during app updates  
✅ Migration happens automatically without user intervention  
✅ Original database preserved until migration success confirmed  
✅ Clear logging for troubleshooting  
✅ Graceful failure handling (app still starts)  
✅ Performance acceptable for typical datasets (<10 seconds)
