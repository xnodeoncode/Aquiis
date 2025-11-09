# Aquiis - Revision History

## November 8, 2025

### PDF Document Generation System

**QuestPDF Integration**

- ✅ Installed QuestPDF 2025.7.4 package for professional PDF generation
- ✅ Configured Community License for the application
- ✅ Created comprehensive PDF generator services for leases, invoices, and payments

**PDF Generator Classes Created:**

1. **LeasePdfGenerator.cs** - Generates professional lease agreements

   - Property information section (address, city, state, type, beds/baths)
   - Tenant information section (name, email, phone)
   - Lease terms section (start/end dates, duration, status)
   - Financial terms section (monthly rent, security deposit, total rent)
   - Additional terms section (custom terms if present)
   - Signature blocks for landlord and tenant
   - Professional formatting with Letter-size pages and 2cm margins
   - Page numbering in footer

2. **InvoicePdfGenerator.cs** - Generates professional invoices

   - Header with invoice number, dates, and color-coded status badge
   - Bill To section (tenant information)
   - Property information section
   - Invoice details table with description and amounts
   - Payment history table (if payments exist)
   - Financial summary (invoice total, paid amount, balance due)
   - Notes section if applicable
   - Color-coded status indicators (Green=Paid, Red=Overdue, Orange=Pending, Blue=Partially Paid)

3. **PaymentPdfGenerator.cs** - Generates payment receipts
   - Prominent "PAID" badge header
   - Large, centered amount paid display
   - Payment details (date, method, transaction reference)
   - Complete invoice information showing balance after payment
   - Tenant and property information sections
   - Notes section if applicable
   - "Thank you for your payment" footer message

**Integration Points:**

- ✅ Added "Generate PDF" button to ViewLease.razor with loading spinner
- ✅ Added "Generate PDF" button to ViewInvoice.razor (replaced Print Invoice)
- ✅ Added "Generate Receipt" button to ViewPayment.razor
- ✅ All generated PDFs automatically saved to Documents table
- ✅ PDFs properly associated with lease, property, and tenant
- ✅ Auto-navigation to lease documents page after generation
- ✅ Success notifications via JavaScript alerts

### Document Management Enhancement

**View in Browser Functionality**

- ✅ Added `viewFile()` JavaScript function using Blob URLs
- ✅ Updated `fileDownload.js` with proper blob handling
- ✅ Converts base64 data to Blob for clean URL generation
- ✅ Opens documents in new browser tab with native viewer (PDF viewer, image viewer, etc.)
- ✅ Automatic cleanup of blob URLs to prevent memory leaks
- ✅ Added "View" button (eye icon) alongside Download button in LeaseDocuments.razor

**LeaseDocuments.razor Updates:**

- Document viewing with three action buttons: View | Download | Delete
- View button opens document in new tab using blob URL
- Download button saves file to local system
- Proper MIME type handling for all file types

### Documents Page Redesign

**Complete UI/UX Overhaul for Documents.razor**

- ✅ Redesigned to match Invoices.razor pattern for consistency
- ✅ Shows all documents from last 30 days regardless of type
- ✅ Removed separate panels for different document types

**New Features:**

1. **Advanced Filtering:**

   - Search box for filename and description
   - Document type dropdown filter (Lease Agreement, Invoice, Payment Receipt, Addendum, Inspections, Insurance, Agreements, Correspondence, Notice, Other)
   - "Group by Property" toggle switch
   - "Clear Filters" button

2. **Summary Cards:**

   - Lease Agreements count (Primary/Blue)
   - Invoices count (Warning/Yellow)
   - Payment Receipts count (Success/Green)
   - Total Documents count (Info/Light Blue)

3. **Dual View Modes:**

   **Grouped by Property View:**

   - Collapsible property sections (click to expand/collapse)
   - Shows property address and location
   - Document count per property
   - Full document details table per property
   - Includes lease information (tenant, period)
   - Expandable/collapsible headers

   **Flat List View:**

   - Sortable columns (Document, Type, Uploaded Date)
   - Shows all documents in single table
   - Displays property, lease, and tenant columns
   - Pagination controls (20 items per page)
   - Click column headers to sort

4. **Document Actions:**

   - **View** - Opens document in browser tab (eye icon)
   - **Download** - Downloads the file (download icon)
   - **View Lease** - Navigate to associated lease (file-text icon, shown if lease exists)

5. **Additional Features:**
   - Color-coded badges for document types
   - File icons based on extension (PDF=red, Word=blue, Image=green, etc.)
   - Shows file size, upload date, and uploader name
   - Handles documents without property/lease associations
   - Responsive table layouts
   - Professional styling consistent with application theme

**Technical Implementation:**

- Client-side filtering and sorting for better performance
- Proper state management with expandedProperties HashSet
- Efficient grouping using LINQ GroupBy
- Pagination with page size of 20 documents
- Case-insensitive search functionality
- Proper null handling for optional associations

### Bug Fixes and Improvements

**Build Errors Resolved:**

- ✅ Fixed `invoice.LeaseId.HasValue` error in ViewInvoice.razor (changed to `invoice.LeaseId > 0`)
- ✅ Fixed namespace ambiguity in LeasePdfGenerator.cs (changed `Document.Create` to `QuestPDF.Fluent.Document.Create`)
- ✅ Resolved view document blank page issue by implementing proper Blob URL approach
- ✅ All components compile successfully

**Code Quality:**

- Added proper using directives for Documents namespace across view pages
- Implemented consistent error handling with try-catch-finally blocks
- Added loading states (isGenerating) for all PDF generation operations
- Proper async/await patterns throughout
- Comprehensive null checking for navigation properties

### File Structure Changes

**New Files Created:**

```
Aquiis.WebUI/
├── Components/
│   └── PropertyManagement/
│       └── Documents/
│           ├── InvoicePdfGenerator.cs (NEW)
│           ├── LeasePdfGenerator.cs (NEW)
│           └── PaymentPdfGenerator.cs (NEW)
└── wwwroot/
    └── js/
        └── fileDownload.js (UPDATED - added viewFile function)
```

**Modified Files:**

```
Aquiis.WebUI/
├── Components/
│   └── PropertyManagement/
│       ├── Documents/
│       │   └── Pages/
│       │       ├── Documents.razor (MAJOR REDESIGN)
│       │       └── LeaseDocuments.razor (Added View functionality)
│       ├── Invoices/
│       │   └── Pages/
│       │       └── ViewInvoice.razor (Added PDF generation)
│       ├── Leases/
│       │   └── Pages/
│       │       └── ViewLease.razor (Added PDF generation)
│       └── Payments/
│           └── Pages/
│               └── ViewPayment.razor (Added PDF generation)
└── Aquiis.WebUI.csproj (Added QuestPDF package reference)
```

### User Experience Improvements

**Document Generation Workflow:**

1. Create business records (Lease, Invoice, or Payment)
2. View the record detail page
3. Click "Generate PDF" or "Generate Receipt" button
4. System generates professional PDF document
5. PDF automatically saved to Documents table with proper associations
6. User redirected to lease documents page
7. Document appears in Documents.razor page (if within 30 days)

**Document Viewing Workflow:**

1. Navigate to Documents page or LeaseDocuments page
2. Click "View" button (eye icon) on any document
3. Document opens in new browser tab with native viewer
4. PDF files display in browser's PDF viewer
5. Images display directly in browser
6. Clean blob URL in address bar (no base64 clutter)

**Document Organization:**

- Documents automatically categorized by type
- Grouped by property for better organization
- Filtered by lease for contextual viewing
- Search and filter for quick access
- Recent documents (30 days) highlighted on main Documents page

### Technical Notes

**Dependencies:**

- QuestPDF 2025.7.4 (Community License)
- QuestPDF.Fluent
- QuestPDF.Helpers
- QuestPDF.Infrastructure

**Browser Compatibility:**

- Blob URL support for modern browsers
- PDF viewing requires browser PDF viewer plugin
- Fallback download option always available
- Tested with Chrome, Edge, Firefox

**Performance Considerations:**

- Binary document storage in SQLite database
- Base64 encoding for JavaScript transfers
- Blob URLs for memory-efficient viewing
- Automatic blob cleanup after viewing
- Pagination for large document lists
- 10MB file upload limit in LeaseDocuments

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
