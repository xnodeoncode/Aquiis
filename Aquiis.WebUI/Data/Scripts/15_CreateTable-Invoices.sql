CREATE TABLE IF NOT EXISTS "Invoices" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "LeaseId" INTEGER NOT NULL,
    "UserId" TEXT NOT NULL,
    "InvoiceNumber" TEXT NOT NULL UNIQUE,
    "InvoiceDate" TEXT NOT NULL,
    "DueDate" TEXT NOT NULL,
    "Amount" REAL NOT NULL,
    "Description" TEXT,
    "PaidDate" TEXT,
    "PaidAmount" REAL,
    "Status" TEXT NOT NULL DEFAULT 'Pending', --Pending, Paid, Overdue, Cancelled
    "Notes" TEXT,
    "CreatedDate" TEXT NOT NULL DEFAULT (datetime('now')),
    "CreatedBy" TEXT NOT NULL DEFAULT '',
    "LastModified" TEXT,
    "LastModifiedBy" TEXT NOT NULL DEFAULT '',
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY("LeaseId") REFERENCES "Leases"("Id")
);